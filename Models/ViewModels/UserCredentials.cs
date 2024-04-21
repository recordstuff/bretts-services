namespace bretts_services.Models.ViewModels;

public record UserCredentials
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
