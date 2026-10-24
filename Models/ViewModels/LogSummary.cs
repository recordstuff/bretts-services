namespace bretts_services.Models.ViewModels;

/// <summary>Represents the fields displayed in a page of application logs.</summary>
public record LogSummary
{
    public Guid Guid { get; set; }
    public string? Message { get; set; }
    public LogEventLevel? Level { get; set; }
    public DateTime? TimeStamp { get; set; }
    public string? SourceContext { get; set; }
    public string? ServerName { get; set; }
    public string? Environment { get; set; }
}
