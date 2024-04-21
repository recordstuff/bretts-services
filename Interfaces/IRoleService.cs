using bretts_services.Models.ViewModels;

namespace bretts_services.Interfaces;

public interface IRoleService
{
    Task<List<NameGuidPair>> GetRoles();
}
    