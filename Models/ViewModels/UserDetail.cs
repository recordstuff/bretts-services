namespace bretts_services.Models.ViewModels;

public record UserDetail : UserSummary
{
    public string Phone { get; set; } = string.Empty;
    public List<NameGuidPair> Roles { get; set; } = new();

}
