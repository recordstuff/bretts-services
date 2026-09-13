using bretts_services.Models.ViewModels;

namespace bretts_services.Interfaces;

public interface ILogService
{
    Task<PaginationResult<Entities.Log>> GetLogs(LogSearchParameters searchParameters);
    Task<List<string>> GetAttributes();
    Task<Entities.Log?> GetLog(int id);
    Task<Entities.Log> InsertLog(Entities.Log log);
    Task<Entities.Log?> UpdateLog(Entities.Log log);
    Task<bool> DeleteLog(int id);
}
