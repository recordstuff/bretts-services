namespace bretts_services.Models.LMStudio;

internal sealed class ChatStreamResponse
{
    [JsonPropertyName("choices")]
    public List<ChatStreamChoice> Choices { get; init; } = [];
}
