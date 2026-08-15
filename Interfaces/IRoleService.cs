using bretts_services.Models.ViewModels;

namespace bretts_services.Interfaces;

public interface IRoleService
{
    Task<List<NameGuidPair>> GetRoles();

    Task<PaginationResult<NameGuidPair>> GetRoles(int page, int pageSize, string? searchText,
        RolesSortColumn sortColumn = RolesSortColumn.Name, SortDirection sortDirection = SortDirection.Ascending);

    Task<NameGuidPair?> GetRole(Guid guid);

    Task<RoleSaveResult> InsertRole(RoleNew role);

    Task<RoleSaveResult> UpdateRole(NameGuidPair role);

    Task<RoleSaveResult> DeleteRole(Guid guid);
}
