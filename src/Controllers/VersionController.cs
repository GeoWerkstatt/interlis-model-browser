using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;

namespace ModelRepoBrowser.Controllers;

[ApiController]
[Route("[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, "The current Version of the ModelRepoBrowser application.")]
    public string Get()
    {
        var assembly = typeof(Program).Assembly;
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
            assembly.GetName().Version.ToString();
    }
}
