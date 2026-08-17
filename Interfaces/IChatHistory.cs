using bretts_services.Models.LMStudio;

namespace bretts_services.Interfaces;

public interface IChatHistory
{
    void Add(ChatMessage message);
    IReadOnlyList<ChatMessage> Get();
    void Clear();
}
