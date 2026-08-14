namespace bretts_services.Models.ViewModels;

/// <summary>
/// Identifies a sortable column in the roles list.
/// </summary>
public enum RolesSortColumn
{
    /// <summary>
    /// Sorts by the role's public identifier.
    /// </summary>
    Id = 0,

    /// <summary>
    /// Sorts by the role's name.
    /// </summary>
    Name = 1,
}
