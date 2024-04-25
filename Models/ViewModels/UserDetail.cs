namespace bretts_services.Models.ViewModels;

public record UserDetail : UserSummary
{
    public string? Phone { get; set; }
    public List<NameGuidPair> Roles { get; set; } = new();

}
