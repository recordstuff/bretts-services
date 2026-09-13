using bretts_services.Mappings;
using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;
using System.Text.Json;

namespace bretts_services.Services;

public class LogService : ILogService
{
    private readonly BrettsAppContext _brettsAppContext;
    private readonly LogMapping _logMapping;

    public LogService(BrettsAppContext brettsAppContext, LogMapping logMapping)
    {
        _brettsAppContext = brettsAppContext;
        _logMapping = logMapping;
    }

    public async Task<PaginationResult<LogSummary>> GetLogs(LogSearchParameters searchParameters)
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
                        [Operator] nvarchar(50) '$.Operator',
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
                        ([filter].[Operator] = {nameof(LogFilterOperator.Exists)} AND [property].[key] IS NOT NULL)
                        OR ([filter].[Operator] = {nameof(LogFilterOperator.DoesNotExist)} AND [property].[key] IS NULL)
                        OR ([filter].[Operator] = {nameof(LogFilterOperator.Equals)} AND [property].[value] = [filter].[Value])
                        OR ([filter].[Operator] = {nameof(LogFilterOperator.DoesNotEqual)} AND [property].[value] <> [filter].[Value])
                        OR ([filter].[Operator] = {nameof(LogFilterOperator.Contains)} AND CHARINDEX([filter].[Value], [property].[value]) > 0)
                        OR ([filter].[Operator] = {nameof(LogFilterOperator.DoesNotContain)} AND CHARINDEX([filter].[Value], [property].[value]) = 0)
                        OR ([filter].[Operator] = {nameof(LogFilterOperator.GreaterThan)} AND TRY_CONVERT(decimal(38, 10), [property].[value]) > TRY_CONVERT(decimal(38, 10), [filter].[Value]))
                        OR ([filter].[Operator] = {nameof(LogFilterOperator.GreaterThanOrEqual)} AND TRY_CONVERT(decimal(38, 10), [property].[value]) >= TRY_CONVERT(decimal(38, 10), [filter].[Value]))
                        OR ([filter].[Operator] = {nameof(LogFilterOperator.LessThan)} AND TRY_CONVERT(decimal(38, 10), [property].[value]) < TRY_CONVERT(decimal(38, 10), [filter].[Value]))
                        OR ([filter].[Operator] = {nameof(LogFilterOperator.LessThanOrEqual)} AND TRY_CONVERT(decimal(38, 10), [property].[value]) <= TRY_CONVERT(decimal(38, 10), [filter].[Value]))
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

        if (searchParameters.Level.HasValue)
        {
            var level = _logMapping.ToSerilogLogEventLevel(searchParameters.Level.Value);
            query = query.Where(log => log.Level == level);
        }

        var count = await query.CountAsync();

        if (searchParameters.SortDirection == SortDirection.Descending)
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

        return new PaginationResult<LogSummary>
        {
            Page = searchParameters.Page,
            PageCount = (int)Math.Ceiling((double)count / searchParameters.PageSize),
            ItemCount = count,
            Items = _logMapping.ToLogSummaries(logs),
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

    public async Task<LogDetail?> GetLog(Guid guid)
    {
        var log = await _brettsAppContext.Logs
            .AsNoTracking()
            .SingleOrDefaultAsync(log => log.LogGuid == guid);

        if (log is null)
        {
            return null;
        }

        return _logMapping.ToLogDetail(log);
    }

    public async Task<LogDetail> InsertLog(LogNew logNew)
    {
        var log = _logMapping.ToLog(logNew);
        _brettsAppContext.Logs.Add(log);
        await _brettsAppContext.SaveChangesAsync();
        return _logMapping.ToLogDetail(log);
    }

    public async Task<LogDetail?> UpdateLog(LogDetail logDetail)
    {
        var storedLog = await _brettsAppContext.Logs
            .SingleOrDefaultAsync(candidate => candidate.LogGuid == logDetail.Guid);

        if (storedLog is null)
        {
            return null;
        }

        _logMapping.UpdateLog(logDetail, storedLog);
        await _brettsAppContext.SaveChangesAsync();
        return _logMapping.ToLogDetail(storedLog);
    }

    public async Task<bool> DeleteLog(Guid guid)
    {
        var log = await _brettsAppContext.Logs.SingleOrDefaultAsync(candidate => candidate.LogGuid == guid);

        if (log is null)
        {
            return false;
        }

        _brettsAppContext.Logs.Remove(log);
        await _brettsAppContext.SaveChangesAsync();
        return true;
    }
}
