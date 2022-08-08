using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public string Get(string q)
    {
        logger.LogInformation("Search with query <{SearchQuery}>", q);

        var searchPattern = $"%{EscapeLikePattern(q)}%";
        var models = context.Models
            .Where(m => !m.File.StartsWith("obsolete/"))
            .Where(m =>
                EF.Functions.ILike(m.Name, searchPattern, @"\")
                || EF.Functions.ILike(m.Version, searchPattern, @"\")
                || EF.Functions.ILike(m.File, searchPattern, @"\")
                || m.Tags.Contains(q))
            .Select(m => $"{m.Name}, {m.Version}, {m.File}, {{{string.Join(", ", m.Tags)}}}")
            .ToList();

        return string.Join("\n", models);
    }

    internal string EscapeLikePattern(string pattern)
    {
        return pattern
            .Replace(@"\", @"\\", StringComparison.OrdinalIgnoreCase)
            .Replace("_", @"\_", StringComparison.OrdinalIgnoreCase)
            .Replace("%", @"\%", StringComparison.OrdinalIgnoreCase);
    }
}
