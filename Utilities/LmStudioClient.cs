namespace bretts_services.Utilities;

using bretts_services.Models.LMStudio;

using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;

public sealed class LmStudioClient
{
    private static readonly SemaphoreSlim ModelLock = new(1, 1);
    private static string? _loadedModelKey;
    private static string? _loadedModelInstanceId;

    private readonly HttpClient _httpClient;
    private readonly IChatHistory _chatHistory;

    public LmStudioClient(HttpClient httpClient, IChatHistory chatHistory)
    {
        _httpClient = httpClient;
        _chatHistory = chatHistory;
    }

    public async IAsyncEnumerable<string> ChatAsync(string prompt)
    {
        var loadedModel = await GetLoadedModelReferenceAsync();

        var request = new ChatRequest
        {
            Model = loadedModel.InstanceId,
            Messages =
            [
                new ChatMessage
                {
                    Role = "user",
                    Content = prompt
                }
            ],
            Temperature = 0.2,
            MaxTokens = 1000,
            Stream = true
        };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/v1/chat/completions")
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead);

        await EnsureSuccessAsync(response);

        var sb = new StringBuilder();

        using var stream =
            await response.Content.ReadAsStreamAsync();

        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync() is { } line)
        {
            if (!line.StartsWith("data: "))
            {
                continue;
            }

            var data = line[6..];

            if (data == "[DONE]")
            {
                if (sb.Length > 0)
                {
                    _chatHistory.Add(new ChatMessage { Role = "assistant", Content = $"{sb}" });
                }

                yield break;
            }

            var chunk =
                JsonSerializer.Deserialize<ChatStreamResponse>(data);

            var content = chunk?
                .Choices
                .FirstOrDefault()?
                .Delta?
                .Content;
            if (!string.IsNullOrEmpty(content))
            {
                sb.Append(content);
                yield return content;
            }
        }
    }

    public async Task<string> GetLoadedModelAsync()
    {
        var loadedModel = await GetLoadedModelReferenceAsync();
        return loadedModel.ModelKey;
    }

    public async Task<IReadOnlyList<string>> GetAvailableModelsAsync()
    {
        var models = await GetModelsAsync();

        return models
            .Where(IsLanguageModel)
            .Select(model => model.Key)
            .OfType<string>()
            .Where(model => !string.IsNullOrWhiteSpace(model))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(model => model, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<string?> ChangeLoadedModelAsync(string model)
    {
        var modelKey = model.Trim();

        await ModelLock.WaitAsync();

        try
        {
            var models = await GetModelsAsync();
            var requestedModel = models.FirstOrDefault(availableModel =>
                IsLanguageModel(availableModel)
                && string.Equals(availableModel.Key, modelKey, StringComparison.Ordinal));

            if (requestedModel == null)
            {
                return null;
            }

            var requestedInstance = requestedModel.LoadedInstances
                .FirstOrDefault(instance => !string.IsNullOrWhiteSpace(instance.Id));
            var currentModel = FindCurrentLoadedModel(models);

            if (requestedInstance?.Id is not null
                && string.Equals(requestedInstance.Id, currentModel?.InstanceId, StringComparison.Ordinal))
            {
                _loadedModelKey = modelKey;
                _loadedModelInstanceId = requestedInstance.Id;
                return _loadedModelKey;
            }

            if (currentModel is not null)
            {
                await UnloadModelAsync(currentModel.InstanceId);
                _loadedModelKey = null;
                _loadedModelInstanceId = null;
            }

            if (requestedInstance?.Id is not null)
            {
                _loadedModelKey = modelKey;
                _loadedModelInstanceId = requestedInstance.Id;
                return _loadedModelKey;
            }

            _loadedModelInstanceId = await LoadModelAsync(modelKey);
            _loadedModelKey = modelKey;
            return _loadedModelKey;
        }
        finally
        {
            ModelLock.Release();
        }
    }

    private async Task<IReadOnlyList<LmStudioModel>> GetModelsAsync()
    {
        using var response = await _httpClient.GetAsync("/api/v1/models");
        await EnsureSuccessAsync(response);

        var modelResponse = await response.Content
            .ReadFromJsonAsync<LmStudioModelsResponse>();

        if (modelResponse == null)
        {
            throw new InvalidOperationException(
                "LM Studio returned an empty model list response.");
        }

        return modelResponse.Models;
    }

    private async Task UnloadModelAsync(string instanceId)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "/api/v1/models/unload",
            new LmStudioUnloadModelRequest { InstanceId = instanceId });

        await EnsureSuccessAsync(response);
    }

    private async Task<string> LoadModelAsync(string model)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "/api/v1/models/load",
            new LmStudioLoadModelRequest { Model = model });

        await EnsureSuccessAsync(response);

        var loadResponse = await response.Content
            .ReadFromJsonAsync<LmStudioLoadModelResponse>();

        if (loadResponse == null || string.IsNullOrWhiteSpace(loadResponse.InstanceId))
        {
            throw new InvalidOperationException(
                "LM Studio did not return the loaded model instance ID.");
        }

        return loadResponse.InstanceId;
    }

    private async Task<LoadedModelReference> GetLoadedModelReferenceAsync()
    {
        if (_loadedModelKey is not null && _loadedModelInstanceId is not null)
        {
            return new LoadedModelReference(_loadedModelKey, _loadedModelInstanceId);
        }

        await ModelLock.WaitAsync();

        try
        {
            if (_loadedModelKey is not null && _loadedModelInstanceId is not null)
            {
                return new LoadedModelReference(_loadedModelKey, _loadedModelInstanceId);
            }

            var models = await GetModelsAsync();
            var loadedModel = FindCurrentLoadedModel(models);

            if (loadedModel is null)
            {
                throw new InvalidOperationException(
                    "No language model is loaded in LM Studio.");
            }

            _loadedModelKey = loadedModel.ModelKey;
            _loadedModelInstanceId = loadedModel.InstanceId;

            return loadedModel;
        }
        finally
        {
            ModelLock.Release();
        }
    }

    private static LoadedModelReference? FindCurrentLoadedModel(
        IReadOnlyList<LmStudioModel> models)
    {
        if (_loadedModelInstanceId is not null)
        {
            foreach (var model in models.Where(IsLanguageModel))
            {
                var cachedInstance = model.LoadedInstances.FirstOrDefault(instance =>
                    string.Equals(instance.Id, _loadedModelInstanceId, StringComparison.Ordinal));

                if (cachedInstance?.Id is not null && !string.IsNullOrWhiteSpace(model.Key))
                {
                    return new LoadedModelReference(model.Key, cachedInstance.Id);
                }
            }
        }

        foreach (var model in models.Where(IsLanguageModel))
        {
            var loadedInstance = model.LoadedInstances
                .FirstOrDefault(instance => !string.IsNullOrWhiteSpace(instance.Id));

            if (loadedInstance?.Id is not null && !string.IsNullOrWhiteSpace(model.Key))
            {
                return new LoadedModelReference(model.Key, loadedInstance.Id);
            }
        }

        return null;
    }

    private static bool IsLanguageModel(LmStudioModel model)
    {
        return string.Equals(model.Type, "llm", StringComparison.Ordinal);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var error = await response.Content.ReadAsStringAsync();

        throw new HttpRequestException(
            $"LM Studio returned {(int)response.StatusCode} {response.StatusCode}: {error}");
    }

    private sealed record LoadedModelReference(string ModelKey, string InstanceId);
}
