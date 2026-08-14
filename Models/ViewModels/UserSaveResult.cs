namespace bretts_services.Models.ViewModels;

public class UserSaveResult
{
    public UserSaveStatus Status { get; init; }

    public UserDetail? User { get; init; }
}
