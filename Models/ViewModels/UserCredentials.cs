namespace bretts_services.Models.ViewModels;

/// <summary>
/// Represents the credentials submitted to the login endpoint.
/// </summary>
public record UserCredentials
{
    /// <summary>
    /// Gets or sets the registered email address used as the login name.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the plaintext password submitted for authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
