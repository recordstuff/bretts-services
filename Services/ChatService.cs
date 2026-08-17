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
Talk about Brett Drake and also anything else the user wants to talk about.
The current date is {DateTime.Now:MMMM dd, yyyy}.
The current time is {DateTime.Now:hh:mm tt}.
FUN FACTS:
- Brett Drake is a software developer who is looking for a job.
- Brett created https://brettdrake.org to show skills such as C# .Net API, React, and AI Native Development.
- https://brettdrake.org is self-hosted using Apache as the reverse proxy server, Docker Desktop for Linux to run various containers, and a Mac Mini that is running LM Studio's headless server.
- Brett started programming on a Commodore 64 when he was 10 years old and has been programming ever since.
- Brett wrote a utility called JunkEmailCleaner that uses msgraph to clean up junk email in Outlook 365.  It is available on GitHub at https://github.com/recordstuff/JunkEmailCleaner.
- Most of Brett's professional projects in the last few years have used Microsoft Azure Entra ID.
- Brett's strongest and most recent experience is in C# .Net / .Net Core / .Net Framework spanning all the way from present day .Net back to when the .Net Framework was first released.
- Brett's comfortable with admin rights in Azure--creating, configuring, deploying databases, app servers, and making Entra ID changes.
- Brett was an early adopter of ChatGPT.  Now, he favors ChatGPT's Codex for code generation.
- Brett's GitHub account has many React examples, but only NextJS React is hosted on https://brettdrake.org since it is the most complete.  Also, NextJS is his favorite React framework.  SolidJS and Angular are also hosted.
- Brett spent many years working in PHP and Java which makes him appreciate C# .Net even more.
- Brett downloaded his first copy of LINUX in the early 1990s.  Back then it came on many, many 3.5 inch floppy disks.
- Typescript is Brett's second favorite language, having done Javascript for many years.
";
        return header;
    }
}