namespace bretts_services.Models.Entities;

public partial class Role
{
    public long RoleID { get; set; }

    public Guid RoleGuid { get; set; }

    [StringLength(64)]
    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
