namespace bretts_services.Models.ViewModels;

/// <summary>
/// Represents the public identity fields used to summarize a user.
/// </summary>
public record UserSummary
{
    /// <summary>
    /// Gets or sets the user's public identifier.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}
