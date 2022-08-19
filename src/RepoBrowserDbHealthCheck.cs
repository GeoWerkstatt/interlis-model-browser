using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ModelRepoBrowser;

/// <summary>
/// Health check for model repository database. Checks whether the database is accessible and contains data.
/// </summary>
public class RepoBrowserDbHealthCheck : IHealthCheck
{
    private readonly RepoBrowserContext repoBrowserContext;

    public RepoBrowserDbHealthCheck(RepoBrowserContext context)
    {
        repoBrowserContext = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
    {
        var canConnectToDb = await repoBrowserContext.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false);
        var dbContainsData = false;

        if (canConnectToDb)
        {
            dbContainsData = repoBrowserContext.Models.Any() && repoBrowserContext.Repositories.Any() && repoBrowserContext.Catalogs.Any();
        }

        if (canConnectToDb && dbContainsData)
        {
            return HealthCheckResult.Healthy();
        }
        else
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}
