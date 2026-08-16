using System.Text.Json;

namespace bretts_services.Services;

public sealed class SessionChatHistory : IChatHistory
{
    private const string SessionKey = "ChatHistory";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionChatHistory(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session
        ?? throw new InvalidOperationException("No active HTTP session.");

    public void Add(string message)
    {
        var messages = Get().ToList();
        messages.Add(message);

        Session.SetString(
            SessionKey,
            JsonSerializer.Serialize(messages));
    }

    public IReadOnlyList<string> Get()
    {
        var json = Session.GetString(SessionKey);

        return string.IsNullOrWhiteSpace(json)
            ? []
            : JsonSerializer.Deserialize<List<string>>(json) ?? [];
    }

    public void Clear()
    {
        Session.Remove(SessionKey);
    }
}