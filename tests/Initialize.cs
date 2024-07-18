using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModelRepoBrowser;

[TestClass]
public sealed class Initialize
{
    [AssemblyInitialize]
    public static void AssemplyInitialize(TestContext testContext)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        using var context = ContextFactory.CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Clear database and fill it with test data
        context.SearchQueries.ExecuteDelete();
        context.Catalogs.ExecuteDelete();
        context.Models.ExecuteDelete();
        context.Repositories.ExecuteDelete();
        context.InterlisFiles.ExecuteDelete();
        context.SaveChanges();

        context.SeedData();
    }
}
