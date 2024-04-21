namespace bretts_services.Models.ViewModels;

public record UserSummary
{
    public Guid Guid { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
