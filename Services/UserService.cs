using Microsoft.Extensions.Options;

namespace bretts_services.Services;

public class UserService : IUserService
{
    private readonly BrettsAppContext _brettsAppContext;
    private readonly UserOptions _userOptions;

    public UserService(BrettsAppContext brettsAppContext, IOptions<UserOptions> options)
    {
        _brettsAppContext = brettsAppContext;
        _userOptions = options.Value;
    }

    public async Task<string> Login(UserCredentials userCredintials)
    {
        userCredintials.Email = userCredintials.Email.ToLower();

        var user = await _brettsAppContext.Users
            .Include("Roles")
            .FirstOrDefaultAsync(u => u.Email.ToLower() == userCredintials.Email);

        if (user is null) return string.Empty;

        if (!Hashing.Verify(userCredintials.Password, user.Password, user.Salt)) return string.Empty;

        var roles = user.Roles.Select(r => r.Name).ToList();

        return JwtHelper.GetJwtToken(user.Email, user.DisplayName ?? user.Email, _userOptions.SigningKey, _userOptions.Issuer, _userOptions.Audience, roles);
    }

    public async Task<bool> Add(UserCredentials userCredintials)
    {
        userCredintials.Email = userCredintials.Email.ToLower();

        var user = await _brettsAppContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == userCredintials.Email);
        
        if (user is not null)
        {
            return false;
        }

        user = new User();

        user.Email = userCredintials.Email;

        user.Password = Hashing.Hash(userCredintials.Password, out var salt);

        user.Salt = salt;

        _brettsAppContext.Users.Add(user);
        var written = await _brettsAppContext.SaveChangesAsync();

        return written != 0;
    }
}
