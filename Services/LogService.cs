using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;
using System.Text.Json;

namespace bretts_services.Services;

public class LogService : ILogService
{
    private readonly BrettsAppContext _brettsAppContext;

    public LogService(BrettsAppContext brettsAppContext)
    {
        _brettsAppContext = brettsAppContext;
    }

    public async Task<PaginationResult<Entities.Log>> GetLogs(LogSearchParameters searchParameters)
    {
        var attributeFiltersJson = JsonSerializer.Serialize(searchParameters.AttributeFilters);
        var query = _brettsAppContext.Logs
            .FromSqlInterpolated($"""
                SELECT [log].*
                FROM [Logs] AS [log]
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM OPENJSON({attributeFiltersJson})
                    WITH
                    (
                        [Attribute] nvarchar(4000) '$.Attribute',
                        [Operator] int '$.Operator',
                        [Value] nvarchar(max) '$.Value'
                    ) AS [filter]
                    OUTER APPLY
                    (
                        SELECT TOP (1) [property].[key], [property].[value]
                        FROM OPENJSON
                        (
                            CASE WHEN ISJSON([log].[LogEvent]) = 1 THEN [log].[LogEvent] END
                        ) AS [property]
                        WHERE [property].[key] = [filter].[Attribute]
                    ) AS [property]
                    WHERE NOT
                    (
                        ([filter].[Operator] = 0 AND [property].[key] IS NOT NULL)
                        OR ([filter].[Operator] = 1 AND [property].[key] IS NULL)
                        OR ([filter].[Operator] = 2 AND [property].[value] = [filter].[Value])
                        OR ([filter].[Operator] = 3 AND [property].[value] <> [filter].[Value])
                        OR ([filter].[Operator] = 4 AND [property].[value] LIKE CONCAT('%', [filter].[Value], '%'))
                        OR ([filter].[Operator] = 5 AND [property].[value] NOT LIKE CONCAT('%', [filter].[Value], '%'))
                        OR ([filter].[Operator] = 6 AND TRY_CONVERT(decimal(38, 10), [property].[value]) > TRY_CONVERT(decimal(38, 10), [filter].[Value]))
                        OR ([filter].[Operator] = 7 AND TRY_CONVERT(decimal(38, 10), [property].[value]) >= TRY_CONVERT(decimal(38, 10), [filter].[Value]))
                        OR ([filter].[Operator] = 8 AND TRY_CONVERT(decimal(38, 10), [property].[value]) < TRY_CONVERT(decimal(38, 10), [filter].[Value]))
                        OR ([filter].[Operator] = 9 AND TRY_CONVERT(decimal(38, 10), [property].[value]) <= TRY_CONVERT(decimal(38, 10), [filter].[Value]))
                    )
                )
                """)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchParameters.SearchText))
        {
            var searchText = searchParameters.SearchText.Trim();
            query = query.Where(log =>
                (log.Message != null && log.Message.Contains(searchText))
                || (log.MessageTemplate != null && log.MessageTemplate.Contains(searchText))
                || (log.Exception != null && log.Exception.Contains(searchText))
                || (log.LogEvent != null && log.LogEvent.Contains(searchText)));
        }

        if (searchParameters.From.HasValue)
        {
            query = query.Where(log => log.TimeStamp >= searchParameters.From.Value);
        }

        if (searchParameters.To.HasValue)
        {
            query = query.Where(log => log.TimeStamp <= searchParameters.To.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchParameters.Level))
        {
            query = query.Where(log => log.Level == searchParameters.Level);
        }

        var count = await query.CountAsync();

        if (searchParameters.NewestFirst)
        {
            query = query.OrderByDescending(log => log.TimeStamp).ThenByDescending(log => log.Id);
        }
        else
        {
            query = query.OrderBy(log => log.TimeStamp).ThenBy(log => log.Id);
        }

        var logs = await query
            .Skip(searchParameters.PageSize * (searchParameters.Page - 1))
            .Take(searchParameters.PageSize)
            .ToListAsync();

        return new PaginationResult<Entities.Log>
        {
            Page = searchParameters.Page,
            PageCount = (int)Math.Ceiling((double)count / searchParameters.PageSize),
            ItemCount = count,
            Items = logs,
        };
    }

    public Task<List<string>> GetAttributes()
    {
        return _brettsAppContext.Database
            .SqlQueryRaw<string>("""
                SELECT DISTINCT [property].[key] AS [Value]
                FROM [Logs] AS [log]
                CROSS APPLY OPENJSON
                (
                    CASE WHEN ISJSON([log].[LogEvent]) = 1 THEN [log].[LogEvent] END
                ) AS [property]
                WHERE [property].[key] NOT LIKE '@%'
                ORDER BY [Value]
                """)
            .ToListAsync();
    }

    public Task<Entities.Log?> GetLog(int id)
    {
        return _brettsAppContext.Logs.AsNoTracking().SingleOrDefaultAsync(log => log.Id == id);
    }

    public async Task<Entities.Log> InsertLog(Entities.Log log)
    {
        log.Id = 0;
        _brettsAppContext.Logs.Add(log);
        await _brettsAppContext.SaveChangesAsync();
        return log;
    }

    public async Task<Entities.Log?> UpdateLog(Entities.Log log)
    {
        var storedLog = await _brettsAppContext.Logs.SingleOrDefaultAsync(candidate => candidate.Id == log.Id);

        if (storedLog is null)
        {
            return null;
        }

        storedLog.Message = log.Message;
        storedLog.MessageTemplate = log.MessageTemplate;
        storedLog.Level = log.Level;
        storedLog.TimeStamp = log.TimeStamp;
        storedLog.Exception = log.Exception;
        storedLog.LogEvent = log.LogEvent;
        storedLog.SourceContext = log.SourceContext;
        storedLog.ServerName = log.ServerName;
        storedLog.Environment = log.Environment;
        await _brettsAppContext.SaveChangesAsync();
        return storedLog;
    }

    public async Task<bool> DeleteLog(int id)
    {
        var log = await _brettsAppContext.Logs.SingleOrDefaultAsync(candidate => candidate.Id == id);

        if (log is null)
        {
            return false;
        }

        _brettsAppContext.Logs.Remove(log);
        await _brettsAppContext.SaveChangesAsync();
        return true;
    }
}
