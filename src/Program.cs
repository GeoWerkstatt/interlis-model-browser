using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using ModelRepoBrowser;
using ModelRepoBrowser.Crawler;
using Npgsql.Logging;
using System.Reflection;
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
builder.Services.AddSwaggerGen(options =>
{
    // Include existing documentation in Swagger UI.
    options.IncludeXmlComments(
        Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

    // Custom order in Swagger UI.
    options.OrderActionsBy(apiDescription =>
    {
        var customOrder = new[] { "Search", "Model", "Version" };
        var controllerName = (apiDescription.ActionDescriptor as ControllerActionDescriptor)?.ControllerName;
        return $"{Array.IndexOf(customOrder, controllerName)}";
    });

    options.EnableAnnotations();
    options.SupportNonNullableReferenceTypes();

});

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
app.UseSwagger();
app.UseSwaggerUI();

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
