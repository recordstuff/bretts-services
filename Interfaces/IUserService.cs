namespace bretts_services.Interfaces;

public interface IUserService
{
    Task<string> Login(UserCredentials userCredintials);

    Task<bool> Add(UserCredentials userCredintials);

    Task<PaginationResult<DisplayedUser>> GetUsers(int page, int pageSize, string? searchText, Roles roleFilter);
}
    