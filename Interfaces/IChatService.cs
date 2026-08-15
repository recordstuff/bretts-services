namespace bretts_services.Interfaces;

public interface IChatService
{
    IAsyncEnumerable<string> ChatAsync(string prompt);
}