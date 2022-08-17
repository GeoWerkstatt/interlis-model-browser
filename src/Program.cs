using Microsoft.EntityFrameworkCore;
using ModelRepoBrowser;
using ModelRepoBrowser.Crawler;
using Npgsql.Logging;

NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Debug, true, false);
NpgsqlLogManager.IsParameterLoggingEnabled = true;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("RepoBrowserContext");
builder.Services.AddNpgsql<RepoBrowserContext>(connectionString, options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

builder.Services.AddTransient<IRepositoryCrawler, RepositoryCrawler>().AddHttpClient();
builder.Services.AddHostedService<DbUpdateService>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

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
