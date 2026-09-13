namespace bretts_services.Models.ViewModels;

/// <summary>
/// Represents the complete editable details and role assignments for a user.
/// </summary>
public record UserDetail : UserInput
{
    /// <summary>
    /// Gets or sets the user's public identifier.
    /// </summary>
    public Guid Guid { get; set; }
}
