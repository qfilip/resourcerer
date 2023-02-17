using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Logic;
using System.Reflection;

namespace Resourcerer.Api.Services;
public static partial class ServiceRegistry
{
    public static void AddAppServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddDbContext<AppDbContext>(cfg =>
            cfg.UseSqlite(AppInitializer.GetDbConnection(env)));
        
        services.AddAuth();
        services.AddSwagger();
        services.AddAuthorization();
    }

    public static void Add3rdParyServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            var assembly = typeof(IResourcererLogicAssemblyMarker).GetTypeInfo().Assembly;
            cfg.RegisterServicesFromAssembly(assembly);
        });
    }
}

