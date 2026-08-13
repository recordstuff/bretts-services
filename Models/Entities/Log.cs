namespace bretts_services.Models.Entities;

/// <summary>
/// Represents a persisted structured application log entry.
/// </summary>
public class Log
{
    /// <summary>
    /// Gets or sets the database identifier for the log entry.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the rendered log message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the original structured message template.
    /// </summary>
    public string? MessageTemplate { get; set; }

    /// <summary>
    /// Gets or sets the Serilog severity level.
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// Gets or sets the date and time at which the event was recorded.
    /// </summary>
    public DateTime? TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the serialized exception details when the event records an exception.
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// Gets or sets the serialized structured log event.
    /// </summary>
    public string? LogEvent { get; set; }

    /// <summary>
    /// Gets or sets the component or class that emitted the event.
    /// </summary>
    public string? SourceContext { get; set; }

    /// <summary>
    /// Gets or sets the host name of the server that emitted the event.
    /// </summary>
    public string? ServerName { get; set; }

    /// <summary>
    /// Gets or sets the application environment that emitted the event.
    /// </summary>
    public string? Environment { get; set; }
}
