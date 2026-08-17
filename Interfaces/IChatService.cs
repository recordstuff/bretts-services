namespace bretts_services.Interfaces;

public interface IChatService
{
    Task<string> GetLoadedModelAsync();
    IAsyncEnumerable<string> ChatAsync(string prompt);
}