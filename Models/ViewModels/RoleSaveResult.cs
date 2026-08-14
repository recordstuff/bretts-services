namespace bretts_services.Models.ViewModels;

public class RoleSaveResult
{
    public RoleSaveStatus Status { get; init; }

    public NameGuidPair? Role { get; init; }
}
