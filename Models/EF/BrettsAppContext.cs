namespace bretts_services.Models.EF;

public partial class BrettsAppContext : DbContext
{
    public BrettsAppContext(DbContextOptions<BrettsAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
}
