namespace bretts_services.Models.ViewModels;

/// <summary>Represents an application log entry exposed by the API.</summary>
public record LogDetail : LogSummary
{
    public string? MessageTemplate { get; set; }
    public string? Exception { get; set; }
    public string? LogEvent { get; set; }
}
