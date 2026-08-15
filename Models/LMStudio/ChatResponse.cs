namespace bretts_services.Models.LMStudio;

internal sealed class ChatResponse
{
    [JsonPropertyName("choices")]
    public List<Choice> Choices { get; init; } = [];
}
