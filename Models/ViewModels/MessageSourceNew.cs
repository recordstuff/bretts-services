namespace bretts_services.Models.ViewModels;

/// <summary>
/// Represents a new message source to store in the JunkEmailCleaner database.
/// </summary>
public record MessageSourceNew
{
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
