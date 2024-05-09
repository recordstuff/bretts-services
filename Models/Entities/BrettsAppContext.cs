namespace bretts_services.Models.Entities;

public partial class BrettsAppContext : DbContext
{
    public BrettsAppContext(DbContextOptions<BrettsAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.HasIndex(e => e.TimeStamp, "IX1_Logs");

            entity.Property(e => e.Environment).IsUnicode(false);
            entity.Property(e => e.ServerName).IsUnicode(false);
            entity.Property(e => e.SourceContext).IsUnicode(false);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(e =>
        {
            e.HasIndex(e => e.Name)
                .IsUnique();

            e.Property(r => r.RoleGuid)
                .HasDefaultValueSql("NEWID()");

            e.HasData(
                new Role { RoleID = 1, Name = "Admin", RoleGuid = new Guid("cdf2beff-ea73-4d8b-9fe8-33818e52776f") },
                new Role { RoleID = 2, Name = "User", RoleGuid = new Guid("111224ad-f6a4-4ca1-ade2-2e6ab407d8e8") }
            );
        });


        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email)
                .IsUnique();

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
        ValidateChanges();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override int SaveChanges()
    {
        return SaveChanges(true);
    }
}
