namespace bretts_services.Models.LMStudio;

internal sealed class ChatMessage
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }
}