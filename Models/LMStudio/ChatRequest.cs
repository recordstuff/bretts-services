namespace bretts_services.Models.LMStudio;

internal sealed class ChatRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("messages")]
    public required List<ChatMessage> Messages { get; init; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; init; } = 0.3;

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; init; } = 150;

    [JsonPropertyName("stream")]
    public bool Stream { get; init; } = false;

    [JsonPropertyName("top_p")]
    public double TopP { get; init; } = 1.0;

    [JsonPropertyName("frequency_penalty")]
    public double FrequencyPenalty { get; init; } = 0.0;

    [JsonPropertyName("presence_penalty")]
    public double PresencePenalty { get; init; } = 0.0;

    [JsonPropertyName("stop")]
    public string[]? Stop { get; init; }

    [JsonPropertyName("seed")]
    public int? Seed { get; init; }

    [JsonPropertyName("response_format")]
    public object? ResponseFormat { get; init; }

    [JsonPropertyName("tools")]
    public object[]? Tools { get; init; }

    [JsonPropertyName("tool_choice")]
    public object? ToolChoice { get; init; }
}