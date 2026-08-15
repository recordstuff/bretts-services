namespace bretts_services.Interfaces;

public interface IChatService
{
    Task<string?> ChatAsync(string prompt);
}