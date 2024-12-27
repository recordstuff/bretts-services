# .Net Core 8 API Backend

This project currently serves both a [React frontend](http://brettdrake.org) | [repo](https://github.com/recordstuff/bretts-app)
and an [Angular frontend](http://brettdrake.org:8008) | [repo](https://github.com/recordstuff/bretts-angular).

It runs in a docker container and hits a MS SQL database running in a different docker container.

## Points of Interest

- [Program.cs](https://github.com/recordstuff/bretts-services/blob/master/Program.cs) where CORS, Auth, Jwt generation, global exception handling, and db access are set up.
- [JwtHelper.cs](https://github.com/recordstuff/bretts-services/blob/master/Utilities/JwtHelper.cs) for Jwt creation.
- [Hashing.cs](https://github.com/recordstuff/bretts-services/blob/master/Utilities/Hashing.cs) for reading and writing passwords

## Setting up a MS SQL Server docker image

```
docker pull mcr.microsoft.com/mssql/server

docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=placeholder' -p 1433:1433 -d mcr.microsoft.com/mssql/server
```