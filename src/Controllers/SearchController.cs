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

        var models = context.Models
            .Where(p => p.SearchVector.Matches(EF.Functions.WebSearchToTsQuery("simple", q)))
            .Select(m => $"{m.Name}, {m.Version}, {m.File}, {{{string.Join(", ", m.Tags)}}}")
            .ToList();

        return string.Join("\n", models);
    }
}
