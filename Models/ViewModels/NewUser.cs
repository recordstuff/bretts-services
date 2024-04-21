namespace bretts_services.Models.ViewModels;

public record NewUser : UserCredentials
{
    public string? DisplayName { get; set; }
}
