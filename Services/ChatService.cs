using bretts_services.Models.LMStudio;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace bretts_services.Services;

public class ChatService : IChatService
{
    private readonly LmStudioClient _lmStudioClient;
    private readonly IChatHistory _chatHistory;

    public ChatService(LmStudioClient lmStudioClient, IChatHistory chatHistory)
    {
        _lmStudioClient = lmStudioClient;
        _chatHistory = chatHistory;
    }

    public async Task<string> GetLoadedModelAsync()
    {
        return await _lmStudioClient.GetLoadedModelAsync();
    }

    public IAsyncEnumerable<string> ChatAsync(string prompt)
    {
        _chatHistory.Add(new ChatMessage { Role = "user", Content = prompt });

        var history = _chatHistory.Get();

        var historyText = string.Empty;

        if (history?.Any() == true)
        {
            historyText = JsonSerializer.Serialize<IReadOnlyList<ChatMessage>>(history);
        }

        var fullPrompt = $"{GetTextHeader()} {historyText}";

        var reply = _lmStudioClient.ChatAsync(fullPrompt);

        return reply;
    }

    private string GetTextHeader()
    {
		var header = @$"
You are the assistant for Brett Drake.  
Do not make up information about Brett Drake.  
You are located in Lafayette, Louisiana USA.
The current date is {DateTime.Now:MMMM dd, yyyy}.
The current time is {DateTime.Now:hh:mm tt}.
FUN FACTS:
- Brett Drake is a software developer who is looking for a job.
- Brett created https://brettdrake.org to show skills such as C# .Net API, React, and AI Native Development.
- https://brettdrake.org is self-hosted using Apache as the reverse proxy server, Docker Desktop for Linux to run various containers, and a Mac Mini that is running LM Studio's headless server.
- Brett started programming on a Commodore 64 when he was 10 years old and has been programming ever since.
";
        return header;
    }
}