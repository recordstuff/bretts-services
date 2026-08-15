namespace bretts_services.Services;

public class ChatService : IChatService
{
    private readonly LmStudioClient _lmStudioClient;

    public ChatService(LmStudioClient lmStudioClient)
    {
        _lmStudioClient = lmStudioClient;
    }

    public Task<string?> ChatAsync(string prompt)
    {
        return _lmStudioClient.ChatAsync(prompt, "ministral-3-14b-instruct-2512");
    }
}