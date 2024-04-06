namespace bretts_services.Models.User;

public record NewUser : UserCredentials
{
    public string? DisplayName { get; set; }
}
