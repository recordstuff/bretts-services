# .Net Core 8 API Backend

## AI Native Development Is Here
Make sure to check out where the context is given to each prompt.  This contains the session's conversation history as well as my job history and fun facts.  Context is critical to AI Native Dev, so I'm going to keep thinking about this.

[Giving Context to AI](https://github.com/recordstuff/bretts-services/blob/master/Services/ChatService.cs) |
[The Client That Hits The llmstr AI Server](https://github.com/recordstuff/bretts-services/blob/master/Utilities/LmStudioClient.cs) |
[Try the AI Chat out at https://brettdrake.org](https://brettdrake.org).

## Lacking History?
This project was recreated without history a while back to remove a day's worth of Codex work when I decided not to turn this and the react frontend into an inventory application.  The initial commit contains the old db password and signing key as I used to just check those in since this is a sandbox.  Now, the project uses secrets (as I should have done from the beginning to make the project as real as possible).  The values in the initial commit have been changed and never checked in--all should be secure now.

## What We Serve
This serves three applications:

| Application | Repo |
| ----------- | ---- |
| [React NextJS Demo](https://nextjs.brettdrake.org) | [NextJS Repo](https://github.com/recordstuff/bretts-next) |
| [SolidJS Demo](https://solidjs.brettdrake.org) | [SolidJS Repo](https://github.com/recordstuff/bretts-solid) |
| [Angular Demo](https://angular.brettdrake.org) | [Angular Repo](https://github.com/recordstuff/bretts-angular) |

[brettdrake.org](https://brettdrake.org)

## Points of Interest

- [Program.cs](https://github.com/recordstuff/bretts-services/blob/master/Program.cs) where CORS, Auth, Jwt generation, global exception handling, and db access are set up.
- [JwtHelper.cs](https://github.com/recordstuff/bretts-services/blob/master/Utilities/JwtHelper.cs) for Jwt creation.
- [Hashing.cs](https://github.com/recordstuff/bretts-services/blob/master/Utilities/Hashing.cs) for reading and writing passwords

## Setting up a MS SQL Server docker image

```
docker pull mcr.microsoft.com/mssql/server

docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=<sa password goes here>' -p 1433:1433 -d mcr.microsoft.com/mssql/server
```

## Secrets

The application reads its operational database connection string and JWT signing key from secret providers instead of its tracked appsettings files.

For local development, set both values from the project directory with .NET User Secrets:

```
dotnet user-secrets set "ConnectionStrings:BrettsDbConnection" "<complete-sql-server-connection-string>"
dotnet user-secrets set "UserOptions:SigningKey" "<base64-jwt-signing-key>"
```

For the production Docker Compose deployment, create these two files on the deployment machine:

```
secrets/ConnectionStrings__BrettsDbConnection
secrets/UserOptions__SigningKey
```

Each file must contain only its secret value. The `secrets` directory is excluded from Git and the Docker build context. Docker Compose mounts the files under `/run/secrets`, and ASP.NET Core maps the double underscores in their names to configuration section separators.

Changing the database password file does not change the SQL Server login password. Coordinate that separate SQL Server change with the deployment of the matching secret.

## Building the DB for the first time

First, create the db using Managment Studio.
Then:

```
dotnet ef database update --context BrettsAppContext --startup-project .\bretts-services.csproj --project .\bretts-services.csproj --connection "DataSource=tcp:192.168.0.235,1433;Database=bretts-app;User ID=sa;Password=<sa password goes here>;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" --verbose
```
