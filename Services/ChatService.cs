using System.Net.NetworkInformation;

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

    public IAsyncEnumerable<string> ChatAsync(string prompt)
    {
        var model = "mistral-7b-instruct-v0.1";

        _chatHistory.Add($"From User: {prompt}");

        var history = _chatHistory.Get();

        var fullPrompt = $"{GetTextHeader()}{string.Join("\n", history)}";

        var reply = _lmStudioClient.ChatAsync(fullPrompt, model);

        _chatHistory.Add($"From AI Assistant: {reply}");

        return reply;
    }

    private string GetTextHeader()
    {
		var header = @$"
You are the AI Assistant for https://brettdrake.org for Brett Bish Drake (or just Brett or Brett Drake) so when you see text below labeled AI Assistant: that is you.  these are your privous replies in this same session.  
The text marked User: is the person you are interacting with on Brett's website, brettdrake.org.
Your location is in Lafayette, Louisiana USA.  This is in Acadiana, the home of the Cajuns.  Don't mention this in every reply, and don't repeat yourself about Cajuns.
The current date and time is {DateTime.Now.ToString("G")}
You should mention Brett's need for a job and sell Brett as an employee or contractor, however this should not be done on every reply unless the conversation is directed that way by the User.
You should not make up facts about Brett.  You can speak facts without having to bring Brett into it unless there is something in your prompting including Brett's resume to make this relevant to bring him up.
Also you will see that all of your replies start with AI Assistant: but you do not need to add this to your replies yourself.  It is only there in the history to help you know what you said and what the user said.
Here is the resume of the person you are assisting:
{GetResume()}";
        return header;
    }

	private string GetResume()
	{
		return @"
Brett Drake, Software Developer
(337) 781 - 3074
recordstuff@hotmail.com
I am open to relocation, full time, part time, remote, contract, or permanent work.


http://brettdrake.org
https://github.com/recordstuff
Sample Code: .Net 8 Core API / NextJS React 18 / Vite   React 19 Beta  /  Angular 17 / SolidJS / Kafka


Summary


My strongest and most recent experience is in C# .Net / .Net Core spanning all the way from present day .Net  back to when the .Net Framework was first released.  


My frontend experience recently is in Blazor. Prior to that, it included React,Vue and Angular experience–both Javascript and Typescript projects.  Years prior to that, I used much older web technologies.  Recently I got to do POCs using SolidJS, React Vite, and NextJS.  I’m comfortable with admin rights in Azure–creating, configuring, deploying.  AWS is something I have used for development as well.  Other interests include Docker and AI code generation tools such as Chat GPT’s Codex.


Professional References and Full Resume w/ experience prior to 2011 available on request.


Skill
	Years Experience (Approximate)
	C# .NET Framework, .NET Core, .NET
	15
	MS SQL Server
	6.5
	Entity Framework / Entity Framework Core
	6
	SQL (SQL Server, Postgres, Oracle, MySQL, Maria DB)
	12+ (conservative estimate)
	React
	2.5 years  (initially sole developer of greenfield project)
	Vuejs
	1.5
	Angular
	1
	Javascript
	9.5
	Typescript
	4
	Azure: Service Bus, Deployments (YAML), Azure Kubernetes Service, Cosmos DB, b2c
	1
	Azure DevOps: git, Visual Studio integration
	6
	AWS: SQS, S3 buckets, lambda functions
	1.5
	CSS, LESS, SASS
	10+ (conservative estimate)
	NoSQL (MongoDB, Cosmos DB, Couch DB)
	2+
	jQuery.
	8
	PHP
	7
	Java
	4
	LINUX
	10+ (conservative estimate)
	







EDUCATION


Bachelor of Science in Computer Science
Scientific Option w/ Math minor, 
University of Louisiana at Lafayette, 1997


CERTIFICATION


Lean Agile certified


EXPERIENCE SUMMARY


Courser / Rader (formally CBM)                                                                          01 / 25 - 12 / 25
Developer 2 (formally Senior Developer)
Lafayette, LA Remote                                                   


Do web and desktop development using Microsoft Technologies.  Azure, Blazor Maui Hybrid (Mobile), Blazor Web, SQL Server, and many others.


Responsibilities:
* Estimate hours, write stories, implement stories
* Mobile and website development using MVC, Blazor, Maui iOS, App Connect, Apple Developer.  iOS, APK and Windows mobile.  
* Much Azure server and database creation and maintenance 




AudioScribe                                                                                                 10 / 23 - 04 / 25
Senior Developer
Lafayette, LA Remote                                                   
Originally an employee 20+ years ago.  Then a contractor over the years.
Principal developer of  a product suite I originally wrote beginning around 2003 (copied from an even earlier version of the software).  Software for court reporting and live event captioning using speech recognition, multi track recording, and connections such as internet, modem, and com port.  


Responsibilities:
* Rebuild & rewrite customer database and customer registration website using MVC, Azure app service, MS SQL Server.
* Bring product suite up to Visual Studio 2023 and .Net Framework 4.8 also having tools that use .Net 8
* Code new features and stability improvements for product suite
* C#, C++ ATL COM, VB.net - major components and all new code is in C#




PHI Helipass                                                                                                 03 / 24 - 10 / 24
Senior Developer
Lafayette, LA Remote 


Work off the General Work board including websites in .net 6 and 8 with typescript front ends using KnockoutJS, DDL scripts for Oracle databases, SQL, Entity Framework Core, Xamarin apps, Soap services, WPF and workflow.  Write some green field development using Domain Driven Design and Grpc services.  Also help port existing code to DDD.




Blue Modus                                                                                                7 / 23 - 10 / 23
Senior Developer
Remote
Develop Kentico CMS sites for multiple clients.
Responsibilities:
* Use the Kentico CMS–C# .Net Core, Vue.js
* Use Azure Pipelines to push builds to lower environments
* Work on Azure Build Pipelines and deployment code.
* Integrate Vue.js frontends with the C# / SQL Server CMS backends




Optomi                                                                                                 4 / 23 - 6 / 23
Senior Developer - Contractor  (Short engagement)
Remote
Help integrate corrugated packaging sensors with two ERP systems.
Responsibilities:
* Write Agile stories for ERP integration with packaging sensors.
* Write C# code on Agile team in AWS environment moving to Postgres db




Finexio                                                                                                  5 / 22 - 2 / 23
Senior Developer
Remote 
Developer on Accounts Payable as a service platform. 
Responsibilities: 
* Greenfield development replacing legacy AP as a service system.
* Backend work to write APIs to exactly match legacy responses including design work and standards in the public API, service layer API, and repositories.
* AWS Lambda work to facilitate payments for our partners from buyers to suppliers
* Front end (React, Typescript): Sole initial developer of Admin portal, responsible for project design and mentoring other developers and maintaining code standardization
* Mentoring, reviewing, and cleaning code on C# backend also
* C#, MongoDB, Postgres, React, Typescript, Node JS
* Reading and debugging Python in order to port it.




Perficient                                                                                                 5 / 18 – 5 / 22
Technical Consultant
Lafayette, LA Hybrid
Perficient employee having the following contracting engagements:


CLIENT: Grocery Store Chain 
Moved aspects of website monolith to microservices in Azure, performing  C# .net core / .net 5 and then .net 6 port from .net framework 4.8 and also doing Azure and Kubernetes work (YAML).  Performed customer data related work including aspects of the move to Azure b2c. 


Responsibilities: 
* Account team (full stack but mostly backend) and Microservices team – doing customer profile related changes and then cleaning and moving legacy code to Azure AKS, helping to establish best practices
* Sitecore, C# (move to .net 6), MS SQL Server, Cosmos DB, some Vue.js, styling, begin GraphQL effort. 


CLIENT: National Facility for Computer and Device Repair 
Developed proprietary software for tracking parts, repairs, devices, and customers.
Performed Angular frontend development to continue sprints of legacy codebase fixes and improvements to free client’s developers to be ready for the new codebase for release.
* Angular frontend work (full time frontend Angular role) 


CLIENT: Global Telecom Company 
Developed systems relating to generating quotes and orders for various telecom products. 
Responsibilities: 
* Worked with multiple teams to automate quoting and ordering. 
* Added new products to quoting and ordering API (back-end). 
* Worked with the Team Leader and Architects on green field development (full stack). 
* Provided production support. 
* C#, .NET Core, gRPC, Blazor, Microsoft SQL Server 
* Developed in Jenkins for production deployment use and did deployments. 


CLIENT: Global Telecom Company 
Developed and maintained middleware for product prequalification, pricing, profit analysis, and ordering
Responsibilities: 
* Took over sole development of a large collection of microservices middleware and additional middleware system moving orders across a pipeline involving 7 teams. 
* Provided production support. 
* Coded enhancements in Java, Apache Camel DSL, Oracle 
* DevOps development in Gitlab, Docker, and Kubernetes and provided deployments in test and production environments for the microservices projects and performed Talend deployments for the ordering middleware solution. 


CLIENT: Grocery Store Chain 
Worked on an Agile team developing websites. 
Responsibilities: 
Implemented stories for live public site, admin site, and cooking school admin site (full stack)
Sitecore, C# .NET, MS SQL Server, JavaScript, Less and Sass 


CLIENT: Internal 
Worked on an Agile team developing an employee evaluation system for company-wide use. 
Responsibilities: 
Worked on the UI implementation helping develop the online forms, email notifications and reporting functionalities as dictated by the Human Resources department. 
C# .NET, Microsoft SQL Server, JavaScript, CSS 




Compugistics                                                                                         7 / 11 – 3 / 18 
Senior Developer
Lafayette, LA Hybrid 
Developed various web applications. 
Responsibilities: 
* Gathered project requirements, estimated project hours, developed and supported applications, maintained servers and databases 
* Java, MySQL, MariaDB, JavaScript (jQuery, AngularJS, etc.…), CSS, PHP, CentOS, VMware vSphere, iOS (Objective C) 




Independent Contractor                                                                                    2011 – 2012
Web Developer
Lafayette, LA
 
Responsibilities: 
* Provided schema for an inventory web app where inventory objects had attributes and contained other objects using Entity Framework Code First (C# ASP.NET, MS SQL Server). 
* Assisted in monetizing customers’ VM usage (PHP, LINUX) and Joomla theme customizations 


Prior experience is available on request.




ADDITIONAL TECHNOLOGIES USED PROFESSIONALLY:


Dapr (both the ORM and the microservices framework), gRPC and Blazor (3 month proof of concept, sole developer), Asp.net, MVC, Web forms, Web API 2, VB.net, legacy .net, Docker / Docker Compose, Minikube, Helm, Jenkins, GitLab, Talend, GraphQL proof of concept (Hot Chocolate), Redis, Eclipse, Apache Camel DSL, Spring Boot, Node JS, Angular 1.0, Mustache.js, require.js, etc. Bootstrap, Kestrel, IIS, Tomcat, Apache, Sitecore, Kentico, Wordpress, Joomla, Objective C, Swift, C++, C
";
	}
}