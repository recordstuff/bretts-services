namespace bretts_services.Models.Entities;

public partial class User
{
    public long UserID { get; set; }

    public Guid UserGuid { get; set; }

    [StringLength(256)]
    public string Email { get; set; } = null!;

    [StringLength(64)]
    public byte[] Password { get; set; } = null!;

    [StringLength(64)]
    public byte[] Salt { get; set; } = null!;

    [StringLength(256)]
    public string? DisplayName { get; set; }

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
