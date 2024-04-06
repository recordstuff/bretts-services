namespace bretts_services.Models.User;

public record DisplayedUser
{
    public Guid UserGuid { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
