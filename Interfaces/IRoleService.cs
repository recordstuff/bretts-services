using bretts_services.Models.ViewModels;

namespace bretts_services.Interfaces;

public interface IRoleService
{
    Task<PaginationResult<NameGuidPair>> GetRoles(int page, int pageSize, string? searchText,
        RolesSortColumn sortColumn = RolesSortColumn.Name, SortDirection sortDirection = SortDirection.Ascending);

    Task<NameGuidPair?> GetRole(Guid guid);

    Task<RoleChangeResult> InsertRole(RoleNew role);

    Task<RoleChangeResult> UpdateRole(NameGuidPair role);

    Task<RoleChangeResult> DeleteRole(Guid guid);
}
