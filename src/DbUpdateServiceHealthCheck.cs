using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ModelRepoBrowser;

/// <summary>
/// HealthCheck for <see cref="DbUpdateService"/>.
/// Reports a degraded health if the last model repository database update exited with an exception.
/// </summary>
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
