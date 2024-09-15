using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Mapster;
using MapsterMapper;
using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Resourcerer.Api.Services.StaticServices;
using Resourcerer.Application.Auth.Abstractions;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Swashbuckle.AspNetCore.SwaggerGen;

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
        services.ConfigureOptions<ConfigureSwaggerGenOptions>();
        services.AddApiVersioning(o =>
        {
            o.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            o.ReportApiVersions = true;
            o.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(o =>
        {
            o.GroupNameFormat = "'v'VVV";
            o.FormatGroupName = (group, version) => $"{group} - {version}";
            o.SubstituteApiVersionInUrl = true;
        });

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

    internal class ConfigureSwaggerGenOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }
        public void Configure(string? name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach(var description in _provider.ApiVersionDescriptions)
            {
                var info = new OpenApiInfo()
                {
                    Title = $"Resourcerer.Api v{description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                };

                options.SwaggerDoc(description.GroupName, info);
            }
        }
    }
}
