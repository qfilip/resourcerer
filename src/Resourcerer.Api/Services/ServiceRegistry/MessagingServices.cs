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
using Resourcerer.Dtos.Fake;
using Resourcerer.Dtos.V1;
using Resourcerer.Messaging;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddMessagingServices(IServiceCollection services, IConfiguration configuration)
    {
        var messaging = configuration.GetSection("Messaging");
        var mapFakes = messaging.GetValue<bool>("MapFakes");
        var useChannels = messaging.GetValue<bool>("UseChannels");

        if (useChannels)
        {
            RegisterChannels(services, mapFakes);
        }
        else
        {
            var consumerRegistryFunctions = GetConsumerRegistryFunctions(mapFakes);
            DependencyInjection.AddMassTransit(services, consumerRegistryFunctions);
        }

        DependencyInjection.AddEmailService(services);
    }

    private static void RegisterChannels(IServiceCollection services, bool mapFakes)
    {
        void RegisterSender<TMessage, TSender>(IServiceCollection serives) where TSender : class, IMessageSender<TMessage>
        {
            serives.AddScoped<IMessageSender<TMessage>, TSender>();
        }

        // instance
        DependencyInjection.AddChannelMessagingService<V1InstanceOrderCommand, InstanceOrderEventService>(services);
        RegisterSender<V1InstanceOrderCommand, InstanceOrderCommandSender>(services);

        DependencyInjection.AddChannelMessagingService<V1InstanceDiscardCommand, InstanceDiscardEventService>(services);
        RegisterSender<V1InstanceDiscardCommand, InstanceDiscardCommandSender>(services);
        
        // item
        DependencyInjection.AddChannelMessagingService<V1ItemProductionCommand, ItemProductionOrderEventService>(services);
        RegisterSender<V1ItemProductionCommand, ItemProductionOrderCommandSender>(services);

        if(mapFakes)
        {
            DependencyInjection.AddChannelMessagingService<FakeCommandDto, FakeEventService>(services);
            RegisterSender<FakeCommandDto, FakeCommandSender>(services);
        }
    }

    private static MassTransitConsumersRegistryFunctions[] GetConsumerRegistryFunctions(bool mapFakes)
    {
        // item
        var itemProduction = new MassTransitConsumersRegistryFunctions(
            (IBusRegistrationConfigurator x) =>
            {
                x.AddConsumer<V1CancelCompositeItemProductionOrderCommandConsumer>();
                x.AddConsumer<V1CancelElementItemProductionOrderCommandConsumer>();
                x.AddConsumer<V1CreateCompositeItemProductionOrderCommandConsumer>();
                x.AddConsumer<V1CreateElementItemProductionOrderCommandConsumer>();
                x.AddConsumer<V1FinishItemProductionOrderCommandConsumer>();
                x.AddConsumer<V1StartItemProductionOrderCommandConsumer>();
            },
            (IReceiveEndpointConfigurator q, IBusRegistrationContext ctx) =>
            {
                q.ConfigureConsumer<V1CancelCompositeItemProductionOrderCommandConsumer>(ctx);
                q.ConfigureConsumer<V1CancelElementItemProductionOrderCommandConsumer>(ctx);
                q.ConfigureConsumer<V1CreateCompositeItemProductionOrderCommandConsumer>(ctx);
                q.ConfigureConsumer<V1CreateElementItemProductionOrderCommandConsumer>(ctx);
                q.ConfigureConsumer<V1FinishItemProductionOrderCommandConsumer>(ctx);
                q.ConfigureConsumer<V1StartItemProductionOrderCommandConsumer>(ctx);
            },
            (IInMemoryBusFactoryConfigurator cfg) =>
            {
                cfg.Send<V1CancelCompositeItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderId));
                cfg.Send<V1CancelElementItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderId));
                cfg.Send<V1CreateCompositeItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ItemId));
                cfg.Send<V1CreateElementItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ItemId));
                cfg.Send<V1FinishItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderId));
                cfg.Send<V1StartItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderId));
            },
            () =>
            {
                DependencyInjection.MapCommandEndpoint<V1CancelCompositeItemProductionOrderCommand>();
                DependencyInjection.MapCommandEndpoint<V1CancelElementItemProductionOrderCommand>();
                DependencyInjection.MapCommandEndpoint<V1CreateCompositeItemProductionOrderCommand>();
                DependencyInjection.MapCommandEndpoint<V1CreateElementItemProductionOrderCommand>();
                DependencyInjection.MapCommandEndpoint<V1FinishItemProductionOrderCommand>();
                DependencyInjection.MapCommandEndpoint<V1StartItemProductionOrderCommand>();
            }
        );

        //instance
        var instanceOrder = new MassTransitConsumersRegistryFunctions(
            (IBusRegistrationConfigurator x) =>
            {
                x.AddConsumer<V1InstanceOrderCreateCommandConsumer>();
                x.AddConsumer<V1InstanceOrderCancelCommandConsumer>();
                x.AddConsumer<V1InstanceOrderDeliverCommandConsumer>();
                x.AddConsumer<V1InstanceOrderSendCommandConsumer>();
            },
            (IReceiveEndpointConfigurator q, IBusRegistrationContext ctx) =>
            {
                q.ConfigureConsumer<V1InstanceOrderCreateCommandConsumer>(ctx);
                q.ConfigureConsumer<V1InstanceOrderCancelCommandConsumer>(ctx);
                q.ConfigureConsumer<V1InstanceOrderDeliverCommandConsumer>(ctx);
                q.ConfigureConsumer<V1InstanceOrderSendCommandConsumer>(ctx);
            },
            (IInMemoryBusFactoryConfigurator cfg) =>
            {
                cfg.Send<V1InstanceOrderCreateCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
                cfg.Send<V1InstanceOrderCancelCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
                cfg.Send<V1InstanceOrderDeliverCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
                cfg.Send<V1InstanceOrderSendCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
            },
            () =>
            {
                DependencyInjection.MapCommandEndpoint<V1InstanceOrderCreateCommand>();
                DependencyInjection.MapCommandEndpoint<V1InstanceOrderCancelCommand>();
                DependencyInjection.MapCommandEndpoint<V1InstanceOrderDeliverCommand>();
                DependencyInjection.MapCommandEndpoint<V1InstanceOrderSendCommand>();
            }
        );

        var instanceDiscard = new MassTransitConsumersRegistryFunctions(
            (IBusRegistrationConfigurator x) =>
            {
                x.AddConsumer<V1InstanceDiscardCommandConsumer>();
            },
            (IReceiveEndpointConfigurator q, IBusRegistrationContext ctx) =>
            {
                q.ConfigureConsumer<V1InstanceDiscardCommandConsumer>(ctx);
            },
            (IInMemoryBusFactoryConfigurator cfg) =>
            {
                cfg.Send<V1InstanceDiscardCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
            },
            DependencyInjection.MapCommandEndpoint<V1InstanceDiscardCommand>
        );

        // fakes
        var fakes = new MassTransitConsumersRegistryFunctions(
            (IBusRegistrationConfigurator x) =>
            {
                x.AddConsumer<FakeCommandConsumer>();
            },
            (IReceiveEndpointConfigurator q, IBusRegistrationContext ctx) =>
            {
                q.ConfigureConsumer<FakeCommandConsumer>(ctx);
            },
            (IInMemoryBusFactoryConfigurator cfg) =>
            {
                cfg.Send<FakeCommandDto>(cmd => cmd.UseCorrelationId(_ => Guid.NewGuid()));
            },
            DependencyInjection.MapCommandEndpoint<FakeCommandDto>
        );

        var functions = new List<MassTransitConsumersRegistryFunctions>
        {
            itemProduction,
            instanceOrder,
            instanceDiscard
        };

        if(mapFakes)
            functions.Add(fakes);
        
        return functions.ToArray();
    }
}
