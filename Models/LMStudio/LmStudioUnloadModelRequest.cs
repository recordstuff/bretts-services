namespace bretts_services.Models.LMStudio;

internal sealed class LmStudioUnloadModelRequest
{
    [JsonPropertyName("instance_id")]
    public required string InstanceId { get; init; }
}
