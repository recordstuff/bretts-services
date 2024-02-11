using System.ComponentModel.DataAnnotations;

namespace bretts_services.Models.EF;

public partial class Role
{
    public long RoleID { get; set; }

    public Guid RoleGuid { get; set; }

    [StringLength(64)]
    public string Name { get; set; } = null!;

    public virtual ICollection<User> Roles { get; set; } = new List<User>();
}
