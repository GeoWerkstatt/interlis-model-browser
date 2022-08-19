using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ModelRepoBrowser;

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
