using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Resourcerer.Application.Auth.Abstractions;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void Add3rdParyServices(IServiceCollection services, IWebHostEnvironment env)
    {
        // database
        services.AddDbContext<AppDbContext>(cfg =>
            cfg.UseSqlite(AppInitializer.GetDbConnection(env)));

        // pass user data from jwt to DBContext
        services.AddTransient<AppUser>(x =>
        {
            var service = x.GetRequiredService<IAppIdentityService<AppUser>>();
            if(service == null)
            {
                throw new InvalidOperationException($"{typeof(IAppIdentityService<AppUser>)} not found");
            }

            return service.Get();
        });

        AddMapsterLib(services);
        AddSwagger(services);
    }

    private static void AddMapsterLib(IServiceCollection services)
    {
        // mapster
        // check Mapster.Tool package
        var mapsterConfig = new TypeAdapterConfig();
        void TwoWayMap<TSource, TTarget>()
        {
            mapsterConfig!.NewConfig<TSource, TTarget>();
            mapsterConfig!.NewConfig<TTarget, TSource>();
        }

        TwoWayMap<AppUser, AppUserDto>();
        TwoWayMap<Category, CategoryDto>();
        TwoWayMap<Company, CompanyDto>();
        TwoWayMap<Excerpt, ExcerptDto>();
        TwoWayMap<Instance, InstanceDto>();
        TwoWayMap<Item, ItemDto>();
        TwoWayMap<Price, PriceDto>();
        TwoWayMap<UnitOfMeasure, UnitOfMeasureDto>();

        mapsterConfig.Compile(failFast: true);
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();
    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(o =>
        {
            o.AddSecurityDefinition("bearer", new OpenApiSecurityScheme()
            {
                Name = "Swagger Auth",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }

            });
        });
    }
}
