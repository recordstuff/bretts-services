using bretts_services.Models.LMStudio;
using System.Diagnostics.Contracts;
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

        var fullPrompt = $"{GetTextHeader()} Conversation History: {historyText}";

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
- Brett's email address is recordstuff@hotmail.com and his phone number is (337) 781-3074.
- Brett graduated from the University of Louisiana at Lafayette in 1997 with a Bachelor of Science in Computer Science (Scientific Option w/ Math minor).
- Brett is Lean Agile certified and enjoys working in agile environments.
- Brett's early jobs were using both the C++ and C programming languages.
- In college, Brett wrote a game that was similar to Tron Light Cycles.  It was a two-player game where each player controlled a light cycle that left a trail behind it.  The goal was to make the other player crash into the trail while avoiding crashing into the trail yourself.
- Brett was in the Upsilon Pi Epsilon Honor Society for Computer Science.
- Brett worked on a proof of concept using GraphQL via Hot Chocolate.
- Brett had a professional job before he graduated from college, and that company had color displays but also used much older monochrome displays.

";
        // Job history

        var jobHistory = new List<JobInstance>
        {
            new JobInstance
            {
                Company = "Courser / Rader (Formerly CBM)",
                Title = "Developer 2 (Formerly Senior Developer)",
                Location = "Remote in Lafayette, LA",
                StartMonth = 1,
                StartYear = 2025,
                EndMonth = 12,
                EndYear = 2025,
                Summary = "Mobile and website development using C# MVC, Blazor, Maui iOS, App Connect, Apple Developer. iOS, APK and Windows mobile.  Much Azure app server and database creation and maintenance.",
            },
            new JobInstance
            {
                Company = "PHI Helipass",
                Title = "Senior Developer",
                Location = "Remote in Lafayette, LA",
                StartMonth = 3,
                StartYear = 2024,
                EndMonth = 10,
                EndYear = 2024,
                Summary = "Work on websites in C# .Net with Typescript front ends using KnockoutJS, DDL scripts for Oracle databases, SQL, Entity Framework Core, Xamarin apps, Soap services, GRPC services.",
            },
            new JobInstance
            {
                Company = "Blue Modus",
                Title = "Senior Developer",
                Location = "Remote",
                StartMonth = 7,
                StartYear = 2023,
                EndMonth = 10,
                EndYear = 2023,
                Summary = "Use the Kentico CMS: C# .Net Core, SQL Server, Vue.js.  Use Azure Pipelines to push builds to lower environments.  Work on Azure Build Pipelines and deployment code.",
            },
            new JobInstance
            {
                Company = "Optomi",
                Title = "Senior Developer",
                Location = "Remote",
                StartMonth = 7,
                StartYear = 2023,
                EndMonth = 10,
                EndYear = 2023,
                Summary = "Help integrate corrugated packaging sensors with two ERP systems.  Write C# code on Agile team in AWS environment moving to Postgres db",
            },
            new JobInstance
            {
                Company = "Finexio",
                Title = "Senior Developer",
                Location = "Remote",
                StartMonth = 5,
                StartYear = 2022,
                EndMonth = 2,
                EndYear = 2023,
                Summary = "C# and AWS Developer on Accounts Payable as a service platform. MongoDB, Postgres, React, Typescript, Node JS.  Reading and debugging Python in order to port it to C#.",
            },
            new JobInstance
            {
                Company = "Perficient",
                Title = "Technical Consultant",
                Location = "Hybrid in Lafayette, LA",
                StartMonth = 5,
                StartYear = 2018,
                EndMonth = 5,
                EndYear = 2022,
                Clients = "Pubilx Grocery Stores twice, Lumen (Formerly CenturyLink) twice, National Device Repair Center",
                Summary = "C# API Developer, Java Developer, Angular Developer, MS SQL Server, Oracle, Microservices.",
            },
            new JobInstance
            {
                Company = "Compugistics",
                Title = "Senior Developer",
                Location = "Hybrid in Lafayette, LA",
                StartMonth = 7,
                StartYear = 2011,
                EndMonth = 3,
                EndYear = 2018,
                Summary = "PHP and Java developer hitting MySQL and then MariaDB. jQuery, Angular, LINUX vm creation and configuration.",
            },
            new JobInstance
            {
                Company = "Independent Contractor",
                Title = "Web Developer",
                Location = "Lafayette, LA",
                StartMonth = 1,
                StartYear = 2011,
                EndMonth = 1,
                EndYear = 2012,
                Summary = "Provided schema for an inventory web app where inventory objects had attributes and contained other objects using Entity Framework Code First (C# ASP.NET, MS SQL Server). Assisted in monetizing customers’ VM usage (PHP, LINUX) and Joomla theme customizations.",
            },
        };

        var jobHistoryText = JsonSerializer.Serialize<List<JobInstance>>(jobHistory);

        header += $"{Environment.NewLine} Job History: {jobHistoryText}";

        return header;
    }
}