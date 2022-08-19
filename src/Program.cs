using Microsoft.EntityFrameworkCore;
using ModelRepoBrowser;
using ModelRepoBrowser.Crawler;
using Npgsql.Logging;
using System.Text.Json.Serialization;

NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Debug, true, false);
NpgsqlLogManager.IsParameterLoggingEnabled = true;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("RepoBrowserContext");
builder.Services.AddNpgsql<RepoBrowserContext>(connectionString, options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

builder.Services.AddTransient<IRepositoryCrawler, RepositoryCrawler>().AddHttpClient();
builder.Services.AddHostedService<DbUpdateService>();
builder.Services.AddSingleton<DbUpdateServiceHealthCheck>();

builder.Services.AddHealthChecks()
    .AddCheck<RepoBrowserDbHealthCheck>("RepoBrowserDbHealthCheck")
    .AddCheck<DbUpdateServiceHealthCheck>("DbUpdateServiceHealthCheck");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RepoBrowserContext>();
    context.Database.EnsureCreated();
}

app.Run();
