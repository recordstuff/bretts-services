namespace bretts_services.Utilities;

using bretts_services.Models.LMStudio;

using System.Collections.Generic;
using System.Text.Json;

public sealed class LmStudioClient
{
    private static readonly SemaphoreSlim ModelLock = new(1, 1);
    private static string? _loadedModel;

    private readonly HttpClient _httpClient;
    private readonly IChatHistory _chatHistory;

    public LmStudioClient(HttpClient httpClient, IChatHistory chatHistory)
    {
        _httpClient = httpClient;
        _chatHistory = chatHistory;
    }

    public async IAsyncEnumerable<string> ChatAsync(string prompt)
    {
        var model = await GetLoadedModelAsync();

        var request = new ChatRequest
        {
            Model = model,
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

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new HttpRequestException(
                $"LM Studio returned {(int)response.StatusCode} " +
                $"{response.StatusCode}: {error}");
        }

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
        if (_loadedModel is not null)
        {
            return _loadedModel;
        }

        await ModelLock.WaitAsync();

        try
        {
            if (_loadedModel is not null)
            {
                return _loadedModel;
            }

            using var response =
                await _httpClient.GetAsync("/api/v1/models");

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"LM Studio returned {(int)response.StatusCode} " +
                    $"{response.StatusCode}: {error}");
            }

            using var stream =
                await response.Content.ReadAsStreamAsync();

            using var document =
                await JsonDocument.ParseAsync(stream);

            var loadedModels = new List<string>();

            foreach (var model in document.RootElement
                         .GetProperty("models")
                         .EnumerateArray())
            {
                if (model.GetProperty("type").GetString() != "llm")
                {
                    continue;
                }

                foreach (var instance in model
                             .GetProperty("loaded_instances")
                             .EnumerateArray())
                {
                    var id = instance.GetProperty("id").GetString();

                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        loadedModels.Add(id);
                    }
                }
            }

            _loadedModel = loadedModels.Count switch
            {
                0 => throw new InvalidOperationException(
                    "No language model is loaded in LM Studio."),

                _ => loadedModels[0],
            };

            return _loadedModel;
        }
        finally
        {
            ModelLock.Release();
        }
    }
}