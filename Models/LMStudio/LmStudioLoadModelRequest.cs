namespace bretts_services.Models.LMStudio;

internal sealed class LmStudioLoadModelRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }
}

internal sealed class LmStudioLoadModelResponse
{
    [JsonPropertyName("instance_id")]
    public required string InstanceId { get; init; }
}
