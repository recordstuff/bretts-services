namespace bretts_services.Models.LMStudio;

internal sealed class ChatStreamChoice
{
    [JsonPropertyName("delta")]
    public ChatStreamDelta? Delta { get; init; }
}
