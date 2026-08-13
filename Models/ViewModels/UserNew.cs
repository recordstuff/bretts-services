namespace bretts_services.Models.ViewModels;

/// <summary>
/// Represents the user details, role assignments, and initial password required to create a user.
/// </summary>
public record UserNew : UserDetail
{
    /// <summary>
    /// Gets or sets the plaintext initial password, which is salted and hashed before storage.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
