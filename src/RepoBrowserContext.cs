using Microsoft.EntityFrameworkCore;
using ModelRepoBrowser.Models;

namespace ModelRepoBrowser;

/// <summary>
/// The EF database context containing data for the ModelRepoBrowser application.
/// </summary>
public class RepoBrowserContext : DbContext
{
    public RepoBrowserContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Repository> Repositories { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<Catalog> Catalogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Model>()
            .Property(p => p.SearchVector)
            .HasComputedColumnSql(@"to_tsvector('simple', coalesce(""Name"", '') || ' ' || coalesce(""Version"", '') || ' ' || coalesce(""File"", '')) || array_to_tsvector(""Tags"")", stored: true);

        modelBuilder.Entity<Model>()
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");
    }
}
