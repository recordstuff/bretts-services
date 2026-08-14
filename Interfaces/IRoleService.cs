using bretts_services.Models.ViewModels;

namespace bretts_services.Interfaces;

public interface IRoleService
{
    Task<List<NameGuidPair>> GetRoles();

    Task<NameGuidPair?> GetRole(Guid guid);

    Task<RoleChangeResult> InsertRole(RoleNew role);

    Task<RoleChangeResult> UpdateRole(NameGuidPair role);

    Task<RoleChangeResult> DeleteRole(Guid guid);
}
