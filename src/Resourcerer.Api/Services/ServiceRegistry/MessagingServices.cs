using MassTransit;
using Resourcerer.Api.Services.Messaging.Fake.Channels;
using Resourcerer.Api.Services.Messaging.Fake.MassTransit.Consumers;
using Resourcerer.Api.Services.Messaging.Fake.MassTransit.Senders;
using Resourcerer.Api.Services.Messaging.V1.Channels.Instances;
using Resourcerer.Api.Services.Messaging.V1.Channels.Items;
using Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Instances;
using Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Instances.Discard;
using Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Instances.Production;
using Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Items;
using Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Items.Production;
using Resourcerer.Api.Services.Messaging.V1.MassTransit.Senders;
using Resourcerer.Application.Messaging;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos.Fake;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    // private const string EVENT_CONSUMER_ENDPOINT = "resourcerer";
    private const string COMMAND_CONSUMER_ENDPOINT = "resourcerer";
    public static void AddMessagingServices(IServiceCollection services, IConfiguration configuration)
    {
        var messaging = configuration.GetSection("Messaging");

        var mapFakes = messaging.GetValue<bool>("MapFakes");
        var useChannels = messaging.GetValue<bool>("UseChannels");

        if (useChannels)
        {
            DependencyInjection.AddChannelMessagingService<V1InstanceOrderCommand, InstanceOrderEventService>(services);
            DependencyInjection.AddChannelMessagingService<V1InstanceDiscardCommand, InstanceDiscardEventService>(services);
            DependencyInjection.AddChannelMessagingService<V1ItemProductionCommand, ItemProductionOrderEventService>(services);
            
            if(mapFakes)
                DependencyInjection.AddChannelMessagingService<FakeCommandDto, FakeEventService>(services);

            return;
        }

        RegisterSenders(services, mapFakes);

        var itemProductionFunctions = ItemProductionFunctions.Get();
        var instanceProductionFunction = InstanceProductionFunctions.Get();

        services.AddMassTransit(c =>
        {
            // instance discard
            c.AddConsumer<V1InstanceDiscardCommandConsumer>();

            instanceProductionFunction.AddConsumers(c);
            itemProductionFunctions.AddConsumers(c);

            if (mapFakes)
                c.AddConsumer<FakeCommandConsumer>();

            c.UsingInMemory((ctx, cfg) =>
            {
                cfg.ReceiveEndpoint(COMMAND_CONSUMER_ENDPOINT, q =>
                {
                    // instance discard
                    q.ConfigureConsumer<V1InstanceDiscardCommandConsumer>(ctx);

                    instanceProductionFunction.ConfigureConsumers(q, ctx);
                    itemProductionFunctions.ConfigureConsumers(q, ctx);

                    if (mapFakes)
                        q.ConfigureConsumer<FakeCommandConsumer>(ctx);
                });

                // instance discard
                cfg.Send<V1InstanceDiscardCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));

                instanceProductionFunction.ConfigureCommands(cfg);
                itemProductionFunctions.ConfigureCommands(cfg);

                if (mapFakes)
                    cfg.Send<FakeCommandDto>(cmd => cmd.UseCorrelationId(_ => Guid.NewGuid()));
            });

            // instance discard
            MapCommandEndpoint<V1InstanceDiscardCommand>();

            instanceProductionFunction.MapCommandEndpoints();
            itemProductionFunctions.MapCommandEndpoints();

            if (mapFakes)
                MapCommandEndpoint<FakeCommandDto>();
        });
    }

    public static void MapCommandEndpoint<T>() where T : class
    {
        EndpointConvention.Map<T>(new Uri($"queue:{COMMAND_CONSUMER_ENDPOINT}"));
    }

    private static void RegisterSenders(IServiceCollection services, bool mapFakes)
    {
        // instance
        RegisterSender<V1InstanceDiscardCommand, InstanceDiscardCommandSender>(services);
        RegisterSender<V1InstanceOrderCommand, InstanceOrderCommandSender>(services);

        // item
        RegisterSender<V1ItemProductionCommand, ItemProductionOrderCommandSender>(services);

        if (mapFakes)
            RegisterSender<FakeCommandDto, FakeCommandSender>(services);
    }

    private static void RegisterSender<TMessage, TSender>(IServiceCollection serives)
        where TSender : class, IMessageSender<TMessage>
    {
        serives.AddScoped<IMessageSender<TMessage>, TSender>();
    }
}
