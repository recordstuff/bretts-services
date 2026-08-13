namespace bretts_services.Models.ViewModels;

/// <summary>
/// Represents a display name paired with a public GUID identifier.
/// </summary>
public record NameGuidPair
{
    /// <summary>
    /// Gets or sets the public identifier formatted as a GUID string.
    /// </summary>
    public string Guid { get; set; } = null!;

    /// <summary>
    /// Gets or sets the display name associated with the identifier.
    /// </summary>
    public string Name { get; set; } = null!;
}
