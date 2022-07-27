using Microsoft.EntityFrameworkCore;
using ModelRepoBrowser;
using ModelRepoBrowser.Crawler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("RepoBrowserContext");
builder.Services.AddNpgsql<RepoBrowserContext>(connectionString);

builder.Services.AddTransient<IRepositoryCrawler, RepositoryCrawler>().AddHttpClient();
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

    var crawler = scope.ServiceProvider.GetRequiredService<IRepositoryCrawler>();
    var repositories = crawler.CrawlModelRepositories(new Uri("https://models.interlis.ch")).Result;

    context.Database.BeginTransaction();
    context.Catalogs.RemoveRange(context.Catalogs);
    context.Models.RemoveRange(context.Models);
    context.Repositories.RemoveRange(context.Repositories);

    context.Repositories.AddRange(repositories.Values);

    context.Database.CommitTransaction();
}

new CrawlerScheduler(TimeOnly.FromDateTime(DateTime.Now.AddSeconds(30)));

app.Run();
