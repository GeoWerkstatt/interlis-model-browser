using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ModelRepoBrowser;

public class DbUpdateServiceHealthCheck : IHealthCheck
{
    private volatile bool lastDbUpdateSuccessful;

    public bool LastDbUpdateSuccessful
    {
        get { return lastDbUpdateSuccessful; }
        set { lastDbUpdateSuccessful = value; }
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (lastDbUpdateSuccessful)
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }
        else
        {
            return Task.FromResult(HealthCheckResult.Degraded());
        }
    }
}
