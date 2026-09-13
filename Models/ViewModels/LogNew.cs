namespace bretts_services.Models.ViewModels;

/// <summary>Represents a new application log entry.</summary>
public record LogNew
{
    public string? Message { get; set; }
    public string? MessageTemplate { get; set; }
    public LogEventLevel? Level { get; set; }
    public DateTime? TimeStamp { get; set; }
    public string? Exception { get; set; }
    public string? LogEvent { get; set; }
    public string? SourceContext { get; set; }
    public string? ServerName { get; set; }
    public string? Environment { get; set; }
}
