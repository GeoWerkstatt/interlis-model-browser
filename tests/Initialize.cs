using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        context.SearchQueries.RemoveRange(context.SearchQueries);
        context.Catalogs.RemoveRange(context.Catalogs);
        context.Models.RemoveRange(context.Models);
        context.Repositories.RemoveRange(context.Repositories);
        context.SaveChanges();

        context.SeedData();
    }
}
