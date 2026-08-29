namespace bretts_services.Models.Entities;

/// <summary>
/// Provides access to the database owned and migrated by JunkEmailCleaner.
/// </summary>
public class JunkEmailCleanerContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JunkEmailCleanerContext"/> class.
    /// </summary>
    /// <param name="options">The configured database context options.</param>
    public JunkEmailCleanerContext(DbContextOptions<JunkEmailCleanerContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the stored message sources captured by JunkEmailCleaner.
    /// </summary>
    public DbSet<StoredMessageSource> MessageSources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoredMessageSource>(entity =>
        {
            entity.ToTable("MessageSources");

            entity.HasKey(messageSource => messageSource.MessageSourceId);

            entity.Property(messageSource => messageSource.MessageSourceId)
                .ValueGeneratedOnAdd();

            entity.Property(messageSource => messageSource.BlockedSenderName)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            entity.Property(messageSource => messageSource.MessageSource)
                .HasColumnType("text");
        });

        base.OnModelCreating(modelBuilder);
    }
}
