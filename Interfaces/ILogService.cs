using bretts_services.Models.Entities;

namespace bretts_services.Interfaces;

public interface ILogService
{
    Task<List<Entities.Log>> GetLogs();
}
    