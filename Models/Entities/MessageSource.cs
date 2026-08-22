namespace bretts_services.Models.Entities;

/// <summary>
/// Represents an original email message source captured by JunkEmailCleaner.
/// </summary>
public class MessageSource
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
    /// Gets or sets the complete original message source.
    /// </summary>
    public string? ViewMessageSourceText { get; set; }

    /// <summary>
    /// Gets or sets the unique Microsoft Graph message identifier.
    /// </summary>
    public string GraphMessageId { get; set; } = string.Empty;
}
