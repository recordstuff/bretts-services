using bretts_services.Models.ViewModels;

namespace bretts_services.Interfaces;

public interface IUserService
{
    Task<Login> Login(UserCredentials userCredintials);

    Task<bool> Add(UserNew newUser);

    Task<PaginationResult<UserSummary>> GetUsers(int page, int pageSize, string? searchText, Roles roleFilter);

    Task<UserDetail?> GetUser(Guid guid);

    Task<UserDetail?> InsertUser(UserNew user);
    
    Task<UserDetail?> UpdateUser(UserDetail user);
}
    