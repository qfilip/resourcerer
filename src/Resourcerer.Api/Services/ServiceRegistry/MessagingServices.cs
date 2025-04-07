using MassTransit;
using Resourcerer.Api.Services.Messaging.Channels.V1.Examples;
using Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Examples;
using Resourcerer.Api.Services.Messaging.MassTransit.Senders.V1.Examples;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Messaging;
using DI = Resourcerer.Messaging.DependencyInjection;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddMessagingServices(IServiceCollection services, IConfiguration configuration)
    {
        var messaging = configuration.GetSection("Messaging");
        
        if (messaging == null)
            throw new InvalidOperationException("Messaging configuration not found");

        var useChannels = messaging.GetValue<bool>("UseChannels");

        if (useChannels)
            RegisterChannels(services);
        else
            RegisterMassTransit(services);

        DI.AddEmailService(services);
    }

    private static void RegisterChannels(IServiceCollection services)
    {
        DI.AddChannelMessagingService<
            V1ExampleCommand, ExampleEventService, AppDbContext>(services);
    }

    private static void RegisterMassTransit(IServiceCollection services)
    {
        DI.AddMassTransitSenderService<V1ExampleCommand, ExampleCommandSender>(services);

        var consumerRegistryFunctions = GetConsumerRegistryFunctions();
        DI.AddMassTransit(services, consumerRegistryFunctions);
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
                DI.MapCommandEndpoint<V1CreateExampleCommand>();
                DI.MapCommandEndpoint<V1UpdateExampleCommand>();
            }
        );

        

        var functions = new List<MassTransitConsumersRegistryFunctions>
        {
            examples
        };
        
        return functions.ToArray();
    }
}
