namespace bretts_services.Models.ViewModels;

/// <summary>
/// Identifies the role filter applied when listing users.
/// </summary>
public enum Roles
{
    /// <summary>
    /// Includes users from every role.
    /// </summary>
    Any = 0,

    /// <summary>
    /// Includes only users assigned to the Admin role.
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Includes only users assigned to the User role.
    /// </summary>
    User = 2,
}
