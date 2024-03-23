using Microsoft.EntityFrameworkCore.Metadata;

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
        modelBuilder.Entity<Role>(e =>
        {
            e.Property(r => r.RoleGuid)
                .HasDefaultValueSql("NEWID()");

            e.HasData(
                new Role { RoleID = 1, Name = "Admin", RoleGuid = new Guid("cdf2beff-ea73-4d8b-9fe8-33818e52776f") },
                new Role { RoleID = 2, Name = "User", RoleGuid = new Guid("111224ad-f6a4-4ca1-ade2-2e6ab407d8e8") }
            );
        });


        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.UserGuid)
                .HasDefaultValueSql("NEWID()");

            e.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        });

        base.OnModelCreating(modelBuilder);
    }

    private void ValidateChanges()
    {
        foreach (var entity in ChangeTracker.Entries())
        {
            if (entity.Entity is User user)
            {
                if (entity.State == EntityState.Added 
                 || entity.State == EntityState.Modified)
                {
                    if (!user.Roles.Any())
                    {
                        throw new NotSupportedException("User must have at least one Role.");
                    }
                }
            }
        }
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ValidateChanges();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(true, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override int SaveChanges()
    {
        ValidateChanges();

        return SaveChanges(true);
    }
}
