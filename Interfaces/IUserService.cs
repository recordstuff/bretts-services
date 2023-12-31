namespace bretts_services.Interfaces;

public interface IUserService
{
    string Login(UserCredintials userCredintials);

    void Add(UserCredintials userCredintials);
}
