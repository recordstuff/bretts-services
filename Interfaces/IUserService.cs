namespace bretts_services.Interfaces;

public interface IUserService
{
    Task<string> Login(UserCredintials userCredintials);

    Task<bool> Add(UserCredintials userCredintials);
}
