namespace bretts_services.Models.ViewModels;

/// <summary>
/// Represents the authenticated session returned after a successful login.
/// </summary>
public record LoginSession
{
    /// <summary>
    /// Gets or sets the authenticated user's display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT expiration as seconds since the Unix epoch.
    /// </summary>
    public long ExpirationSeconds { get; set; }

    /// <summary>
    /// Gets or sets the role names included in the JWT claims.
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Gets or sets the signed JWT bearer token used to authorize protected API requests.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
