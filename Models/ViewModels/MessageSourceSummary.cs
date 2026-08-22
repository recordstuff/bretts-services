namespace bretts_services.Models.ViewModels;

/// <summary>
/// Represents a message source without the potentially large original email text.
/// </summary>
public record MessageSourceSummary
{
    /// <summary>
    /// Gets or sets the database identifier.
    /// </summary>
    public long MessageSourceId { get; set; }

    /// <summary>
    /// Gets or sets the blocked sender name exactly as it appeared in the message.
    /// </summary>
    public string? BlockedSenderName { get; set; }

    /// <summary>
    /// Gets or sets the unique Microsoft Graph message identifier.
    /// </summary>
    public string GraphMessageId { get; set; } = string.Empty;
}
