using bretts_services.Models.ViewModels;

namespace bretts_services.Interfaces;

public interface ILogService
{
    Task<PaginationResult<LogSummary>> GetLogs(LogSearchParameters searchParameters);
    Task<List<string>> GetAttributes();
    Task<LogDetail?> GetLog(Guid guid);
    Task<LogDetail> InsertLog(LogDetail logDetail);
    Task<LogDetail?> UpdateLog(LogDetail logDetail);
    Task<bool> DeleteLog(Guid guid);
}
