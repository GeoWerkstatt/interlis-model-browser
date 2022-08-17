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
    /// <returns>
    /// The root of the <see cref="Repository"/> tree or <c>null</c> if no <see cref="Model"/>
    /// matched the <paramref name="query"/>. The Repositories contain only <see cref="Model"/>s
    /// that matched the <paramref name="query"/>.
    /// </returns>
    [HttpGet]
    public async Task<Repository?> Search(string query)
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

        var trimmedQuery = query?.Trim();
        if (string.IsNullOrEmpty(trimmedQuery))
        {
            return null;
        }

        var repositories = await SearchRepositories(trimmedQuery).ConfigureAwait(false);

        // Create repository tree
        foreach (var repository in repositories.Values)
        {
            repository.SubsidiarySites = repository.SubsidiarySites.Select(c => repositories[c.HostNameId]).ToHashSet();
        }

        var root = repositories.Values.SingleOrDefault(r => !r.ParentSites.Any());
        if (root != null && PruneEmptyRepositories(root))
        {
            return root;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get search query suggestions based on <paramref name="query"/>.
    /// </summary>
    /// <param name="query">The query string to search for.</param>
    /// <returns>A sequence of <see cref="Model.Name"/> related to <paramref name="query"/>.</returns>
    [HttpGet("suggest/{query}")]
    public async Task<IEnumerable<string>> GetSearchSuggestions(string query)
    {
        logger.LogDebug("Get search options for <{SearchQuery}>", query);

        var trimmedQuery = query?.Trim();
        if (string.IsNullOrEmpty(trimmedQuery))
        {
            return Enumerable.Empty<string>();
        }

        var repositories = await SearchRepositories(trimmedQuery).ConfigureAwait(false);

        return repositories
            .Values
            .SelectMany(r => r.Models)
            .Select(m => m.Name)
            .ToList();
    }

    private Task<Dictionary<string, Repository>> SearchRepositories(string query)
    {
        var searchPattern = $"%{EscapeLikePattern(query)}%";

        var modelsNamesFoundFromCatalogs = context.Catalogs
            .Where(c => EF.Functions.ILike(c.Identifier, searchPattern, @"\"))
            .Select(c => c.ReferencedModels)
            .AsEnumerable()
            .SelectMany(c => c)
            .Distinct()
            .ToList();

        return context.Repositories
            .Include(r => r.SubsidiarySites)
            .Include(r => r.ParentSites)
            .Include(r => r.Models
                .Where(m => !EF.Functions.ILike(m.File, "obsolete/%"))
                .Where(m =>
                    EF.Functions.ILike(m.Name, searchPattern, @"\")
                    || EF.Functions.ILike(m.Version, searchPattern, @"\")
                    || EF.Functions.ILike(m.File, searchPattern, @"\")
                    || modelsNamesFoundFromCatalogs.Contains(m.Name)
                    || m.Tags.Contains(query)))
            .AsNoTracking()
            .ToDictionaryAsync(r => r.HostNameId);
    }

    /// <summary>
    /// Remove subsidiary repositories that contain no models.
    /// </summary>
    /// <returns><c>true</c> if the <paramref name="repository"/> or any subsidiary repository contains some models, <c>false</c> otherwise.</returns>
    private bool PruneEmptyRepositories(Repository repository)
    {
        repository.SubsidiarySites = repository.SubsidiarySites
            .Where(r => PruneEmptyRepositories(r))
            .ToHashSet();

        return repository.Models.Any() || repository.SubsidiarySites.Any();
    }

    internal string? EscapeLikePattern(string pattern)
    {
        return pattern
            ?.Replace(@"\", @"\\", StringComparison.OrdinalIgnoreCase)
            ?.Replace("_", @"\_", StringComparison.OrdinalIgnoreCase)
            ?.Replace("%", @"\%", StringComparison.OrdinalIgnoreCase);
    }
}
