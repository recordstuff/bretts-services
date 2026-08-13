namespace bretts_services.Models.ViewModels;

/// <summary>
/// Represents the complete editable details and role assignments for a user.
/// </summary>
public record UserDetail : UserSummary
{
    /// <summary>
    /// Gets or sets the user's optional phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the roles assigned to the user.
    /// </summary>
    public List<NameGuidPair> Roles { get; set; } = new();

}
