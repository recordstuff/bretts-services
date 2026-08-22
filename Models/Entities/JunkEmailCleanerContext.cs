namespace bretts_services.Models.Entities;

/// <summary>
/// Provides read-only access to the database owned and migrated by JunkEmailCleaner.
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
    /// Gets the stored message sources captured by JunkEmailCleaner.
    /// </summary>
    public DbSet<MessageSource> MessageSources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MessageSource>(entity =>
        {
            entity.ToTable("MessageSources");

            entity.HasKey(messageSource => messageSource.MessageSourceId);

            entity.HasIndex(messageSource => messageSource.GraphMessageId)
                .IsUnique();

            entity.Property(messageSource => messageSource.MessageSourceId)
                .ValueGeneratedOnAdd();

            entity.Property(messageSource => messageSource.BlockedSenderName)
                .HasColumnType("nvarchar(max)");

            entity.Property(messageSource => messageSource.ViewMessageSourceText)
                .HasColumnType("text");

            entity.Property(messageSource => messageSource.GraphMessageId)
                .HasMaxLength(512)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
