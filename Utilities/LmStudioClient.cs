namespace bretts_services.Utilities;

using bretts_services.Models.LMStudio;

using System.Collections.Generic;
using System.Text.Json;

public sealed class LmStudioClient
{
    private readonly HttpClient _httpClient;

    public LmStudioClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async IAsyncEnumerable<string> ChatAsync(string prompt, string model)
    {
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
            MaxTokens = 300,
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

        using var stream = await response.Content.ReadAsStreamAsync();
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
                yield break;
            }

            var chunk = JsonSerializer.Deserialize<ChatStreamResponse>(data);

            var content = chunk?
                .Choices
                .FirstOrDefault()?
                .Delta?
                .Content;

            if (!string.IsNullOrEmpty(content))
            {
                yield return content;
            }
        }
    }
}
