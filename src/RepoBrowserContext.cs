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
    public DbSet<SearchQuery> SearchQueries { get; set; }
}
