namespace bretts_services.Interfaces;

public interface IChatService
{
    Task<string> GetLoadedModelAsync();
    Task<IReadOnlyList<string>> GetAvailableModelsAsync();
    Task<string?> ChangeLoadedModelAsync(string model);
    IAsyncEnumerable<string> ChatAsync(string prompt);
}
