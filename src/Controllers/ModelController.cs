using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelRepoBrowser.Models;

namespace ModelRepoBrowser.Controllers;

[ApiController]
[Route("[controller]")]
public class ModelController : Controller
{
    private readonly ILogger<ModelController> logger;
    private readonly RepoBrowserContext context;

    public ModelController(ILogger<ModelController> logger, RepoBrowserContext context)
    {
        this.logger = logger;
        this.context = context;
    }

    [HttpGet("{md5}/{name}")]
    public Model? ModelDetails(string md5, string name)
    {
        logger.LogDebug("Get details for Model with hash <{MD5}> and name <{Name}>.", md5, name);

        return context.Models
            .Include(m => m.ModelRepository)
            .Where(m => m.MD5 == md5 && m.Name == name)
            .AsNoTracking()
            .SingleOrDefault();
    }
}
