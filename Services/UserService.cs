using bretts_services.Utilities;

namespace bretts_services.Services;

public class UserService : IUserService
{
    private readonly BrettsAppContext _brettsAppContext;

    public UserService(BrettsAppContext brettsAppContext)
    {
        _brettsAppContext = brettsAppContext;
    }

    public string Login(UserCredintials userCredintials)
    {
        userCredintials.Email = userCredintials.Email.ToUpper();

        var user = _brettsAppContext.Users.FirstOrDefault(u => u.Email.ToUpper() == userCredintials.Email);

        if (user is null) return string.Empty;

        if (!Hashing.Verify(userCredintials.Password, user.Password, user.Salt)) return string.Empty;

        // TODO: not blah blah blah (you are here)

        return JwtHelper.GetJwtToken(user.Email, user.DisplayName ?? user.Email, "blah", "blah", "blah");
    }

    public void Add(UserCredintials userCredintials)
    {
        var user = new User();

        user.Email = userCredintials.Email;

        user.Password = Hashing.Hash(userCredintials.Password, out var salt);

        user.Salt = salt;

        _brettsAppContext.Users.Add(user);
        _brettsAppContext.SaveChanges();
    }
}
