using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;

namespace Resourcerer.DataAccess;

public static class DependencyInjection
{
    public static void RegisterServices(WebApplicationBuilder builder)
    {
        var dbPath = Path.Combine(builder.Environment.WebRootPath, "database.db3");
        var prefix = "Datasource=";

        builder.Services.AddDbContext<AppDbContext>(cfg =>
            cfg.UseSqlite($"{prefix}{dbPath}"));


        // pass identity data from jwt to DBContext
        builder.Services.AddTransient(x =>
        {
            var service = x.GetRequiredService<IAppIdentityService<AppIdentity>>();
            if (service == null)
                throw new InvalidOperationException($"{typeof(IAppIdentityService<AppIdentity>)} not found");

            return service.Get();
        });
    }
}
