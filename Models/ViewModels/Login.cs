namespace bretts_services.Models.ViewModels;

public record Login
{
    public string DisplayName { get; set; } = string.Empty;
    public long ExpirationSeconds { get; set; }
    public List<string> Roles { get; set; } = new();
    public string Token { get; set; } = string.Empty;
}
