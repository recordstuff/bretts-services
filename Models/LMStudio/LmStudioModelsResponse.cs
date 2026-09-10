namespace bretts_services.Models.LMStudio;

internal sealed class LmStudioModelsResponse
{
    [JsonPropertyName("models")]
    public List<LmStudioModel> Models { get; init; } = [];
}

internal sealed class LmStudioModel
{
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("key")]
    public string? Key { get; init; }

    [JsonPropertyName("loaded_instances")]
    public List<LmStudioModelInstance> LoadedInstances { get; init; } = [];
}

internal sealed class LmStudioModelInstance
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }
}
