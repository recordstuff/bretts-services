namespace bretts_services.Models.LMStudio;

public class JobInstance
{
    public string Company { get; set; } = string.Empty;
    public string? Clients { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public int StartMonth { get; set; }
    public int StartYear { get; set; }

    public int? EndMonth { get; set; }
    public int? EndYear { get; set; }

    public string Summary { get; set; } = string.Empty;
}
