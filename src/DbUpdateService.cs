using Microsoft.EntityFrameworkCore;
using ModelRepoBrowser.Crawler;

namespace ModelRepoBrowser;

/// <summary>
/// Service that periodically updates the Model Repository Database using <see cref="IRepositoryCrawler"/>.
/// </summary>
public class DbUpdateService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<DbUpdateService> logger;
    private readonly IConfiguration configuration;

    public TimeOnly PreferedTime { get; set; }

    public DbUpdateService(IServiceScopeFactory scopeFactory, ILogger<DbUpdateService> logger, IConfiguration configuration)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
        this.configuration = configuration;

        PreferedTime = new TimeOnly(1, 0);
    }

    private async Task UpdateModelRepoDatabase()
    {
        logger.LogInformation("Updating ModelRepoDatabase...");

        if (!Uri.TryCreate(configuration["Crawler:RootRepositoryUri"], UriKind.RelativeOrAbsolute, out var rootUri))
        {
            logger.LogError("Unable to parse configuration Crawler:RootRepositoryUri. Database update skipped.");
            return;
        }

        try
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var crawler = scope.ServiceProvider.GetRequiredService<IRepositoryCrawler>();
                var repositories = await crawler.CrawlModelRepositories(rootUri).ConfigureAwait(false);

                using var context = scope.ServiceProvider.GetRequiredService<RepoBrowserContext>();
                context.Database.BeginTransaction();

                context.Catalogs.RemoveRange(context.Catalogs);
                context.Models.RemoveRange(context.Models);
                context.Repositories.RemoveRange(context.Repositories);
                context.SaveChanges();

                context.Repositories.AddRange(repositories.Values);
                context.SaveChanges();

                context.Database.CommitTransaction();
                logger.LogInformation("Updating ModelRepoDatabase complete");
            }
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Unable to update ModelRepoDatabase");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateModelRepoDatabase().ConfigureAwait(false);

            var delayTime = GetTimeSpanUntilPreferedTime(DateTime.Now, PreferedTime);
            logger.LogInformation("Next ModelRepoDatabase update in {DelayTime}", delayTime);
            await Task.Delay(delayTime, stoppingToken).ConfigureAwait(false);
        }
    }

    public static TimeSpan GetTimeSpanUntilPreferedTime(DateTime currentDateTime, TimeOnly preferedTime)
    {
        var preferedDateTime = currentDateTime.Date + preferedTime.ToTimeSpan();
        if (preferedDateTime <= currentDateTime)
        {
            preferedDateTime = preferedDateTime.AddDays(1);
        }

        return preferedDateTime - currentDateTime;
    }
}
