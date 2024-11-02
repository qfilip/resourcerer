using MassTransit;
using Resourcerer.Api.Services.Messaging.Channels.Fake;
using Resourcerer.Api.Services.Messaging.Channels.V1.Instances;
using Resourcerer.Api.Services.Messaging.Channels.V1.Items;
using Resourcerer.Api.Services.Messaging.MassTransit.Consumers.Fake;
using Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Instances.Discard;
using Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Instances.Production;
using Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Items.Production;
using Resourcerer.Api.Services.Messaging.MassTransit.Senders.Fake;
using Resourcerer.Api.Services.Messaging.MassTransit.Senders.V1;
using Resourcerer.Dtos.Fake;
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

        var mapFakes = messaging.GetValue<bool>("MapFakes");
        var useChannels = messaging.GetValue<bool>("UseChannels");

        if (useChannels)
            RegisterChannels(services, mapFakes);
        else
            RegisterMassTransit(services, mapFakes);

        DI.AddEmailService(services);
    }

    private static void RegisterChannels(IServiceCollection services, bool mapFakes)
    {
        // instance
        DI.AddChannelMessagingService<V1InstanceOrderCommand, InstanceOrderEventService>(services);
        DI.AddChannelMessagingService<V1InstanceDiscardCommand, InstanceDiscardEventService>(services);
        DI.AddChannelMessagingService<V1ItemProductionCommand, ItemProductionOrderEventService>(services);

        if(mapFakes)
            DI.AddChannelMessagingService<FakeCommandDto, FakeEventService>(services);
    }
    private static void RegisterMassTransit(IServiceCollection services, bool mapFakes)
    {
        DI.AddMassTransitSenderService<V1InstanceDiscardCommand, InstanceDiscardCommandSender>(services);
        DI.AddMassTransitSenderService<V1InstanceOrderCommand, InstanceOrderCommandSender> (services);
        DI.AddMassTransitSenderService<V1ItemProductionCommand, ItemProductionOrderCommandSender>(services);

        if(mapFakes)
            DI.AddMassTransitSenderService<FakeCommandDto, FakeCommandSender>(services);

        var consumerRegistryFunctions = GetConsumerRegistryFunctions(mapFakes);
        DI.AddMassTransit(services, consumerRegistryFunctions);
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
                DI.MapCommandEndpoint<V1CancelCompositeItemProductionOrderCommand>();
                DI.MapCommandEndpoint<V1CancelElementItemProductionOrderCommand>();
                DI.MapCommandEndpoint<V1CreateCompositeItemProductionOrderCommand>();
                DI.MapCommandEndpoint<V1CreateElementItemProductionOrderCommand>();
                DI.MapCommandEndpoint<V1FinishItemProductionOrderCommand>();
                DI.MapCommandEndpoint<V1StartItemProductionOrderCommand>();
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
                DI.MapCommandEndpoint<V1InstanceOrderCreateCommand>();
                DI.MapCommandEndpoint<V1InstanceOrderCancelCommand>();
                DI.MapCommandEndpoint<V1InstanceOrderDeliverCommand>();
                DI.MapCommandEndpoint<V1InstanceOrderSendCommand>();
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
            DI.MapCommandEndpoint<V1InstanceDiscardCommand>
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
            DI.MapCommandEndpoint<FakeCommandDto>
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
