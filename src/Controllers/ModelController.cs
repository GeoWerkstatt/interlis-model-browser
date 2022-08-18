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

    /// <summary>
    /// Retrieve the Model with the specified <paramref name="md5"/> and <paramref name="name"/>.
    /// </summary>
    /// <param name="md5">The <see cref="Model.MD5"/> of the Model.</param>
    /// <param name="name">The <see cref="Model.Name"/> of the Model.</param>
    /// <returns>The <see cref="Model"/> with the specified <paramref name="md5"/> and <paramref name="name"/> or <c>null</c> if it does not exist.</returns>
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
