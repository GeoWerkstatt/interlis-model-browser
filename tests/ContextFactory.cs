using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRepoBrowser;

internal static class ContextFactory
{
    public static string ConnectionString { get; } = "Host=localhost;Username=postgres;Password=ARKBONES;Database=repobrowser";

    public static RepoBrowserContext CreateContext()
    {
        return new RepoBrowserContext(
            new DbContextOptionsBuilder<RepoBrowserContext>().UseNpgsql(ConnectionString).Options);
    }
}
