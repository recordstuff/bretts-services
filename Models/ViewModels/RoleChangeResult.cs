namespace bretts_services.Models.ViewModels;

public class RoleChangeResult
{
    public RoleChangeStatus Status { get; init; }

    public NameGuidPair? Role { get; init; }
}
