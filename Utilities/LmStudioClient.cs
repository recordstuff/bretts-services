namespace bretts_services.Utilities;

using System.Net.Http.Json;
using System.Text.Json.Serialization;

public sealed class LmStudioClient
{
    private readonly HttpClient _httpClient;

    public LmStudioClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> ChatAsync(
        string prompt,
        string model,
        CancellationToken cancellationToken = default)
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
            ]
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "/v1/chat/completions",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ChatResponse>(
            cancellationToken: cancellationToken);

        return result?.Choices.FirstOrDefault()?.Message?.Content;
    }

    private sealed class ChatRequest
    {
        [JsonPropertyName("model")]
        public required string Model { get; init; }

        [JsonPropertyName("messages")]
        public required List<ChatMessage> Messages { get; init; }
    }

    private sealed class ChatResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; init; } = [];
    }

    private sealed class Choice
    {
        [JsonPropertyName("message")]
        public ChatMessage? Message { get; init; }
    }

    private sealed class ChatMessage
    {
        [JsonPropertyName("role")]
        public string? Role { get; init; }

        [JsonPropertyName("content")]
        public string? Content { get; init; }
    }
}