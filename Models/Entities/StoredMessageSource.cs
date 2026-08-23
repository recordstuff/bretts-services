namespace bretts_services.Models.Entities;

/// <summary>
/// Represents an original email message source captured by JunkEmailCleaner.
/// </summary>
public class StoredMessageSource
{
    /// <summary>
    /// Gets or sets the database identifier.
    /// </summary>
    public long MessageSourceId { get; set; }

    /// <summary>
    /// Gets or sets the blocked sender name exactly as it appeared in the message.
    /// </summary>
    public string BlockedSenderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the complete original message source.
    /// </summary>
    public string? MessageSource { get; set; }
}
