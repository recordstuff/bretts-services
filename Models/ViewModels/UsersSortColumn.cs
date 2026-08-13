namespace bretts_services.Models.ViewModels;

/// <summary>
/// Identifies a sortable column in the users list.
/// </summary>
public enum UsersSortColumn
{
    /// <summary>
    /// Sorts by the user's public identifier.
    /// </summary>
    Id = 0,

    /// <summary>
    /// Sorts by the user's display name.
    /// </summary>
    DisplayName = 1,

    /// <summary>
    /// Sorts by the user's email address.
    /// </summary>
    Email = 2,
}
