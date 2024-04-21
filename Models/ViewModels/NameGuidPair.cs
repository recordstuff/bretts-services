namespace bretts_services.Models.ViewModels;

public record NameGuidPair
{
    public string Guid { get; set; } = null!;
    public string Name { get; set; } = null!;
}
