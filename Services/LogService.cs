using bretts_services.Models.Entities;

namespace bretts_services.Services;

public class LogService : ILogService
{
    private readonly BrettsAppContext _brettsAppContext;

    public LogService(BrettsAppContext brettsAppContext)
    {
        _brettsAppContext = brettsAppContext;
    }

    async public Task<List<Entities.Log>> GetLogs()
    {
        var logs = await _brettsAppContext.Logs
            .AsNoTracking()
            .OrderBy(l => l.TimeStamp)
            .ToListAsync();

        return logs;
    }
}
