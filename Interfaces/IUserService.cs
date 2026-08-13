using bretts_services.Models.ViewModels;

namespace bretts_services.Interfaces;

public interface IUserService
{
    Task<LoginSession> Login(UserCredentials userCredintials);

    Task<PaginationResult<UserSummary>> GetUsers(int page, int pageSize, string? searchText, Roles roleFilter,
        UsersSortColumn sortColumn = UsersSortColumn.DisplayName, SortDirection sortDirection = SortDirection.Ascending);

    Task<UserDetail?> GetUser(Guid guid);

    Task<bool> DeleteUser(Guid guid);

    Task<UserDetail?> InsertUser(UserNew user);
    
    Task<UserDetail?> UpdateUser(UserDetail user);
}
