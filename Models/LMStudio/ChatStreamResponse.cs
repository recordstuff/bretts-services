namespace bretts_services.Models.LMStudio;

internal sealed class ChatStreamResponse
{
    [JsonPropertyName("choices")]
    public List<ChatStreamChoice> Choices { get; init; } = [];
}

public sealed class ChatStreamChoice
{
    [JsonPropertyName("delta")]
    public ChatStreamDelta? Delta { get; init; }
}

public sealed class ChatStreamDelta
{
    [JsonPropertyName("content")]
    public string? Content { get; init; }
}