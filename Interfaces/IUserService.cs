namespace bretts_services.Interfaces;

public interface IUserService
{
    Task<string> Login(UserCredentials userCredintials);

    Task<bool> Add(NewUser newUser);

    Task<PaginationResult<DisplayedUser>> GetUsers(int page, int pageSize, string? searchText, Roles roleFilter);

    Task<DisplayedUser?> GetUser(Guid guid);
}
    