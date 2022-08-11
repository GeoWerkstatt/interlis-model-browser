using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelRepoBrowser.Models;

namespace ModelRepoBrowser.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    private readonly ILogger<SearchController> logger;

    private readonly RepoBrowserContext context;

    public SearchController(ILogger<SearchController> logger, RepoBrowserContext context)
    {
        this.logger = logger;
        this.context = context;
    }

    /// <summary>
    /// Search the Model Repositories for Models that match the <paramref name="query"/>
    /// with their Name, Version, File path or Tags. If the <paramref name="query"/> matches
    /// a catalog name its referenced models are also included in the search result.
    /// </summary>
    /// <param name="query">The query string to search for.</param>
    /// <returns>A sequence of <see cref="Model"/> that matched the <paramref name="query"/>.</returns>
    [HttpGet]
    public async Task<IEnumerable<Model>> Search(string query)
    {
        logger.LogInformation("Search with query <{SearchQuery}>", query);

        try
        {
            context.SearchQueries.Add(new() { Query = query });
            context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Writing query <{SearchQuery}> to the database log failed.", query);
        }

        var searchPattern = $"%{EscapeLikePattern(query)}%";

        var modelsNamesFoundFromCatalogs = context.Catalogs
            .Where(c => EF.Functions.ILike(c.Identifier, searchPattern, @"\"))
            .Select(c => c.ReferencedModels)
            .AsEnumerable()
            .SelectMany(c => c)
            .Distinct()
            .ToList();

        var models = context.Models
            .Include(m => m.ModelRepository)
            .Where(m => !m.File.StartsWith("obsolete/"))
            .Where(m =>
                EF.Functions.ILike(m.Name, searchPattern, @"\")
                || EF.Functions.ILike(m.Version, searchPattern, @"\")
                || EF.Functions.ILike(m.File, searchPattern, @"\")
                || modelsNamesFoundFromCatalogs.Contains(m.Name)
                || m.Tags.Contains(query));

        return await models.AsNoTracking().ToListAsync().ConfigureAwait(false);
    }

    internal string EscapeLikePattern(string pattern)
    {
        return pattern
            .Replace(@"\", @"\\", StringComparison.OrdinalIgnoreCase)
            .Replace("_", @"\_", StringComparison.OrdinalIgnoreCase)
            .Replace("%", @"\%", StringComparison.OrdinalIgnoreCase);
    }
}
