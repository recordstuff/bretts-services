namespace bretts_services.Services;

public class ChatService : IChatService
{
    private readonly LmStudioClient _lmStudioClient;

    public ChatService(LmStudioClient lmStudioClient)
    {
        _lmStudioClient = lmStudioClient;
    }

    public IAsyncEnumerable<string> ChatAsync(string prompt)
    {
        var model = "mistral-7b-instruct-v0.1";

        return _lmStudioClient.ChatAsync(prompt, model);
    }
}