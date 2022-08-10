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

    [HttpGet]
    public async Task<IEnumerable<Model>> Get(string q)
    {
        logger.LogInformation("Search with query <{SearchQuery}>", q);

        context.SearchQueries.Add(new Models.SearchQuery() { Query = q });
        context.SaveChanges();

        var searchPattern = $"%{EscapeLikePattern(q)}%";

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
                || m.Tags.Contains(q));

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
