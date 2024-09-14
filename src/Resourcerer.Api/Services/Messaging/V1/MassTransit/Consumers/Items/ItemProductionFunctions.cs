using MassTransit;
using Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Items.Production;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Items;

public class ItemProductionFunctions
{
    public static (
        Action<IBusRegistrationConfigurator> AddConsumers,
        Action<IInMemoryReceiveEndpointConfigurator, IBusRegistrationContext> ConfigureConsumers,
        Action<IInMemoryBusFactoryConfigurator> ConfigureCommands,
        Action MapCommandEndpoints)
        Get()
    {
        var AddConsumers = (IBusRegistrationConfigurator x) =>
        {
            x.AddConsumer<V1CancelCompositeItemProductionOrderCommandConsumer>();
            x.AddConsumer<V1CancelElementItemProductionOrderCommandConsumer>();
            x.AddConsumer<V1CreateCompositeItemProductionOrderCommandConsumer>();
            x.AddConsumer<V1CreateElementItemProductionOrderCommandConsumer>();
            x.AddConsumer<V1FinishItemProductionOrderCommandConsumer>();
            x.AddConsumer<V1StartItemProductionOrderCommandConsumer>();
        };

        var ConfigureConsumers = (IInMemoryReceiveEndpointConfigurator q, IBusRegistrationContext ctx) =>
        {
            q.ConfigureConsumer<V1CancelCompositeItemProductionOrderCommandConsumer>(ctx);
            q.ConfigureConsumer<V1CancelElementItemProductionOrderCommandConsumer>(ctx);
            q.ConfigureConsumer<V1CreateCompositeItemProductionOrderCommandConsumer>(ctx);
            q.ConfigureConsumer<V1CreateElementItemProductionOrderCommandConsumer>(ctx);
            q.ConfigureConsumer<V1FinishItemProductionOrderCommandConsumer>(ctx);
            q.ConfigureConsumer<V1StartItemProductionOrderCommandConsumer>(ctx);
        };

        var ConfigureCommands = (IInMemoryBusFactoryConfigurator cfg) =>
        {
            cfg.Send<V1CancelCompositeItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderId));
            cfg.Send<V1CancelElementItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderId));
            cfg.Send<V1CreateCompositeItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ItemId));
            cfg.Send<V1CreateElementItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ItemId));
            cfg.Send<V1FinishItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderId));
            cfg.Send<V1StartItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderId));
        };

        var MapCommandEndpoints = () =>
        {
            ServiceRegistry.MapCommandEndpoint<V1CancelCompositeItemProductionOrderCommand>();
            ServiceRegistry.MapCommandEndpoint<V1CancelElementItemProductionOrderCommand>();
            ServiceRegistry.MapCommandEndpoint<V1CreateCompositeItemProductionOrderCommand>();
            ServiceRegistry.MapCommandEndpoint<V1CreateElementItemProductionOrderCommand>();
            ServiceRegistry.MapCommandEndpoint<V1FinishItemProductionOrderCommand>();
            ServiceRegistry.MapCommandEndpoint<V1StartItemProductionOrderCommand>();
        };

        return (AddConsumers, ConfigureConsumers, ConfigureCommands, MapCommandEndpoints);
    }
}
