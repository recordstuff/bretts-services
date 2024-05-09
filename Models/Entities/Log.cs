namespace bretts_services.Models.Entities;

public class Log
{
    public int Id { get; set; }

    public string? Message { get; set; }

    public string? MessageTemplate { get; set; }

    public string? Level { get; set; }

    public DateTime? TimeStamp { get; set; }

    public string? Exception { get; set; }

    public string? LogEvent { get; set; }

    public string? SourceContext { get; set; }

    public string? ServerName { get; set; }

    public string? Environment { get; set; }
}
