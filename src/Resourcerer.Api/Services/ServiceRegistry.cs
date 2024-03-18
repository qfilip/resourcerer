using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Resourcerer.Api.Services.Auth;
using Resourcerer.Api.Services.Messaging;
using Resourcerer.Api.Services.V1_0;
using Resourcerer.DataAccess.AuthService;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances.Events;
using Resourcerer.Dtos.Instances.Events.Order;
using Resourcerer.Logic;
using System.Threading.Channels;

namespace Resourcerer.Api.Services;
public static partial class ServiceRegistry
{
    public static void AddAppServices(this IServiceCollection services)
    {
        RegisterHandlers(typeof(IAppHandler<,>), services);
        RegisterHandlers(typeof(IAppEventHandler<,>), services);

        services.AddScoped<Pipeline>();

        services.AddMessagingService<InstanceOrderEventDtoBase, InstanceOrderEventService>();
        services.AddMessagingService<InstanceDiscardedRequestDto, InstanceDiscardEventService>();
        services.AddMessagingService<ItemProductionEventBaseDto, ItemProductionOrderEventService>();
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
        TwoWayMap<Item, ItemDto>();
        TwoWayMap<Excerpt, ExcerptDto>();
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
        services.AddScoped<AppDbIdentity>();
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

    private static void AddMessagingService<TMessage, TService>(this IServiceCollection services)
        where TService : EventConsumerServiceBase<TMessage>
    {
        services.AddSingleton(_ => Channel.CreateUnbounded<TMessage>());
        services.AddSingleton(sp => sp.GetRequiredService<Channel<TMessage>>().Writer);
        services.AddSingleton(sp => sp.GetRequiredService<Channel<TMessage>>().Reader);

        services.AddSingleton<ISenderAdapter<TMessage>, ChannelSenderService<TMessage>>(sp =>
        {
            var sender = sp.GetRequiredService<Channel<TMessage>>().Writer;
            var consumer = sp.GetRequiredService<Channel<TMessage>>().Reader;
            return new ChannelSenderService<TMessage>(sender);
        });

        services.AddSingleton<IConsumerAdapter<TMessage>, ChannelReaderService<TMessage>>(sp =>
        {
            var consumer = sp.GetRequiredService<Channel<TMessage>>().Reader;
            return new ChannelReaderService<TMessage>(consumer);
        });

        services.AddHostedService<TService>();
    }

    private static void RegisterHandlers(Type handlerType, IServiceCollection services)
    {
        var assembly = handlerType.Assembly;

        var handlers = assembly
            .GetTypes()
            .Where(x =>
                x.GetInterface(handlerType.Name) != null &&
                !x.IsAbstract &&
                !x.IsInterface)
            .ToList();

        handlers.ForEach(x => services.AddTransient(x));
    }
}

