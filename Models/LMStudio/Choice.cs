namespace bretts_services.Models.LMStudio;

internal sealed class Choice
{
    [JsonPropertyName("message")]
    public ChatMessage? Message { get; init; }
}

