namespace bretts_services.Interfaces;

public interface IChatHistory
{
    void Add(string message);
    IReadOnlyList<string> Get();
    void Clear();
}
