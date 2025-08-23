using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Resourcerer.Api.Services;
using Resourcerer.Api.Services.Messaging.Channels.V1.Examples;
using Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Examples;
using Resourcerer.Api.Services.Messaging.MassTransit.Senders.V1.Examples;
using Resourcerer.Api.Services.StaticServices;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Messaging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Resourcerer.Api;

internal static class DependencyInjection
{
    public static void RegisterAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<Pipeline>();
        AddMessagingServices(builder);
        AddSwagger(builder.Services);

        DataAccess.DependencyInjection.RegisterServices(builder);
        Logic.DependencyInjection.RegisterServices(builder);
        Identity.DependencyInjection.RegisterServices(
            builder,
            AppStaticData.Auth.Enabled,
            AppStaticData.Auth.Jwt.Key!,
            AppStaticData.Auth.Jwt.Issuer,
            AppStaticData.Auth.Jwt.Audience,
            AppStaticData.Auth.AuthorizationPolicy.Jwt
        );
        
    }

    public static void AddMessagingServices(WebApplicationBuilder builder)
    {
        var messaging = builder.Configuration.GetSection("Messaging");

        if (messaging == null)
            throw new InvalidOperationException("Messaging configuration not found");

        var useChannels = messaging.GetValue<bool>("UseChannels");

        if (useChannels)
            RegisterChannels(builder.Services);
        else
            RegisterMassTransit(builder.Services);

        Messaging.DependencyInjection.AddEmailService(builder.Services);
    }

    private static void RegisterChannels(IServiceCollection services)
    {
        Messaging.DependencyInjection.AddChannelMessagingService<
            V1ExampleCommand, ExampleEventService, AppDbContext>(services);
    }

    private static void RegisterMassTransit(IServiceCollection services)
    {
        Messaging.DependencyInjection.AddMassTransitSenderService<V1ExampleCommand, ExampleCommandSender>(services);

        var consumerRegistryFunctions = GetConsumerRegistryFunctions();
        Messaging.DependencyInjection.AddMassTransit(services, consumerRegistryFunctions);
    }

    private static MassTransitConsumersRegistryFunctions[] GetConsumerRegistryFunctions()
    {
        var examples = new MassTransitConsumersRegistryFunctions(
            (IBusRegistrationConfigurator x) =>
            {
                x.AddConsumer<V1CreateExampleCommandConsumer>();
                x.AddConsumer<V1UpdateExampleCommandConsumer>();
            },
            (IReceiveEndpointConfigurator q, IBusRegistrationContext ctx) =>
            {
                q.ConfigureConsumer<V1CreateExampleCommandConsumer>(ctx);
                q.ConfigureConsumer<V1UpdateExampleCommandConsumer>(ctx);
            },
            (IInMemoryBusFactoryConfigurator cfg) =>
            {
                cfg.Send<V1CreateExampleCommand>(cmd => cmd.UseCorrelationId(_ => Guid.NewGuid()));
                cfg.Send<V1UpdateExampleCommand>(cmd => cmd.UseCorrelationId(x => x.ExampleId));
            },
            () =>
            {
                Messaging.DependencyInjection.MapCommandEndpoint<V1CreateExampleCommand>();
                Messaging.DependencyInjection.MapCommandEndpoint<V1UpdateExampleCommand>();
            }
        );



        var functions = new List<MassTransitConsumersRegistryFunctions>
        {
            examples
        };

        return functions.ToArray();
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
            foreach (var description in _provider.ApiVersionDescriptions)
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
