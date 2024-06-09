using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;

namespace bretts_services.Utilities;

public static class JwtHelper
{
    // a document on JWT structure: https://datatracker.ietf.org/doc/html/rfc7519

    public static Login GetJwtToken(string email, string displayName, string signingKey, string issuer, string audience, List<Role> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("displayName", displayName),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        var key = new SymmetricSecurityKey(Convert.FromBase64String(signingKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.Add(TimeSpan.FromDays(1));

        var token = new JwtSecurityToken(issuer, audience, claims, null, expires, creds);

        return new Login
        {
             DisplayName = displayName,
             ExpirationSeconds = (long)(DateTime.UnixEpoch - expires).TotalSeconds,
             Roles = roles.Select(r => r.Name).ToList(),
             Token = new JwtSecurityTokenHandler().WriteToken(token),
        };
    }

    public static string? RoleName(Roles role)
    {
        return Enum.GetName(typeof(Roles), role);
    }
}
