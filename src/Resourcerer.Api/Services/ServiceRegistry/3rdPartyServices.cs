using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Resourcerer.Api.Services.StaticServices;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void Add3rdParyServices(IServiceCollection services, IWebHostEnvironment env)
    {
        // database
        services.AddDbContext<AppDbContext>(cfg =>
            cfg.UseSqlite(AppInitializer.GetDbConnection(env)));

        // pass identity data from jwt to DBContext
        services.AddTransient(x =>
        {
            var service = x.GetRequiredService<IAppIdentityService<AppIdentity>>();
            if(service == null)
            {
                throw new InvalidOperationException($"{typeof(IAppIdentityService<AppIdentity>)} not found");
            }

            return service.Get();
        });

        AddMapsterLib(services);
        AddSwagger(services);
    }

    public static TypeAdapterConfig GetMapsterConfig()
    {
        // mapster
        // check Mapster.Tool package
        var mapsterConfig = new TypeAdapterConfig();
        void TwoWayMap<TSource, TTarget>()
        {
            // PreserveReference (prevent stack overflow on recursive nodes)
            mapsterConfig!.NewConfig<TSource, TTarget>().PreserveReference(true); ;
            mapsterConfig!.NewConfig<TTarget, TSource>().PreserveReference(true); ;
        }

        TwoWayMap<ExampleEntity, V1ExampleDto>();

        mapsterConfig.Compile(failFast: true);

        return mapsterConfig;
    }

    private static void AddMapsterLib(IServiceCollection services)
    {
        var mapsterConfig = GetMapsterConfig();
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();
    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.ConfigureOptions<ConfigureSwaggerGenOptions>();
        services.AddApiVersioning(o =>
        {
            //o.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            o.ReportApiVersions = true;
            o.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(o =>
        {
            o.GroupNameFormat = "'v'VVVV";
            //o.FormatGroupName = (group, version) => $"{group}{version}";
            //o.SubstituteApiVersionInUrl = true;
            //o.AddApiVersionParametersWhenVersionNeutral = true;
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
