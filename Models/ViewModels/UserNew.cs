namespace bretts_services.Models.ViewModels;

public record UserNew : UserDetail
{
    public string Password { get; set; } = string.Empty;
}
