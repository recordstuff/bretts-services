namespace bretts_services.Models.EF;

public partial class BrettsAppContext : DbContext
{
    public BrettsAppContext(DbContextOptions<BrettsAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>()
            .Property(r => r.RoleGuid)
            .HasDefaultValueSql("NEWID()");

        modelBuilder.Entity<User>()
            .Property(r => r.UserGuid)
            .HasDefaultValueSql("NEWID()");

        base.OnModelCreating(modelBuilder);
    }
}
