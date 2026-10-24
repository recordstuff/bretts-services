namespace bretts_services.Models.ViewModels;

/// <summary>Represents the editable fields shared by new and existing users.</summary>
public abstract record UserInput
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public List<NameGuidPair> Roles { get; set; } = new();
}
