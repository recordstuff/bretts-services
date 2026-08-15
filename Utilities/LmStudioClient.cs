namespace bretts_services.Utilities;

using bretts_services.Models.LMStudio;

using System.Collections.Generic;
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
            Messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = "user",
                    Content = prompt,
                }
            },
            Temperature = 0.2,
            MaxTokens = 150,
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
}
