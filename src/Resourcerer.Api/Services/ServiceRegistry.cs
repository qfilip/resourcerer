using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Resourcerer.Api.Services.Auth;
using Resourcerer.Messaging;
using Resourcerer.Api.Services.V1;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Abstractions.Services;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using System.Reflection;
using System.Threading.Channels;

namespace Resourcerer.Api.Services;
public static partial class ServiceRegistry
{
    public static void AddAppServices(this IServiceCollection services)
    {
        // handlers and validators
        var handlerTypes = new Type[] { typeof(IAppHandler<,>), typeof(IAppEventHandler<,>) };
        foreach (var handlerType in handlerTypes)
        {
            RegisterHandlers(handlerType, services);
            RegisterValidators(services);
        }

        services.AddScoped<Pipeline>();

        // messaging
        services.AddChannelMessagingService<V1InstanceOrderEvent, InstanceOrderEventService>();
        services.AddChannelMessagingService<V1InstanceDiscardedRequest, InstanceDiscardEventService>();
        services.AddChannelMessagingService<V1ItemProductionEvent, ItemProductionOrderEventService>();

        services.AddScoped<IEmailService, EmailService>();
    }

    public static void AddAspNetServices(this IServiceCollection services)
    {
        services.AddCors(o => o.AddDefaultPolicy(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
        services.AddAuth();
        services.AddSwagger();
        services.AddAuthorization();
    }

    public static void Add3rdParyServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        // database
        services.AddDbContext<AppDbContext>(cfg =>
            cfg.UseSqlite(AppInitializer.GetDbConnection(env)));

        services.AddMapsterLib();
    }

    private static void AddMapsterLib(this IServiceCollection services)
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

    private static void AddSwagger(this IServiceCollection services)
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

    private static void AddAuth(this IServiceCollection services)
    {
        services.AddScoped<AppJwtBearerEvents>();
        services.AddScoped<IAppIdentityService<AppUser>, AppIdentityService>();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = AppStaticData.Auth.Jwt.Issuer,
                ValidAudience = AppStaticData.Auth.Jwt.Audience,
                IssuerSigningKey = AppStaticData.Auth.Jwt.Key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
            };

            o.EventsType = typeof(AppJwtBearerEvents);
        });

        services.AddAuthorization(cfg =>
        {
            cfg.AddPolicy(AppStaticData.Auth.AuthorizationPolicy.Jwt, b =>
                b.RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
        });
    }

    private static void RegisterHandlers(Type handlerType, IServiceCollection services)
    {
        var interfaceName = handlerType.Name;
        var registerAction = (Type x) =>
        {
            services.AddTransient(x);
        };

        RegisterDynamically(interfaceName, registerAction);
    }

    private static void RegisterValidators(IServiceCollection services)
    {
        var interfaceName = typeof(IValidator).Name;
        var registerAction = (Type x) =>
        {
            services.AddSingleton(x);
        };

        RegisterDynamically(interfaceName, registerAction);
    }

    private static void RegisterDynamically(string interfaceName, Action<Type> registerAction)
    {
        var assembly = typeof(Logic.IAssemblyMarker).Assembly;

        var serviceTypes = assembly
        .GetTypes()
        .Where(x =>
            x.GetInterface(interfaceName) != null &&
            !x.IsAbstract &&
            !x.IsInterface)
        .ToList();

        serviceTypes.ForEach(registerAction);
    }
}

