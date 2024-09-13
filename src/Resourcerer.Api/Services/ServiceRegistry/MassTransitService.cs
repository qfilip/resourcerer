using MassTransit;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddMassTransit(IServiceCollection services)
    {
        services.AddMassTransit(c =>
        {
            //c.AddConsumer<>();

            c.UsingInMemory((ctx, cfg) =>
            {
                cfg.ReceiveEndpoint("", q =>
                {
                    // q.ConfigureConsumer<>(ctx);
                });

                // instance discard
                cfg.Send<V1InstanceDiscardCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
                
                // instance order
                cfg.Send<V1InstanceOrderCreateCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
                cfg.Send<V1InstanceOrderCancelCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
                cfg.Send<V1InstanceOrderDeliverCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
                cfg.Send<V1InstanceOrderSendCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
                cfg.Send<V1InstanceDiscardCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));

                // instance production
                cfg.Send<V1CreateItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ItemId));
                cfg.Send<V1CancelItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderEventId));
                cfg.Send<V1StartItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderId));
                cfg.Send<V1FinishItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderId));
            });

            // instance discard
            MapCommandEndpoint<V1InstanceDiscardCommand>();

            // instance order
            MapCommandEndpoint<V1InstanceOrderCreateCommand>();
            MapCommandEndpoint<V1InstanceOrderCancelCommand>();
            MapCommandEndpoint<V1InstanceOrderDeliverCommand>();
            MapCommandEndpoint<V1InstanceOrderSendCommand>();
            MapCommandEndpoint<V1InstanceDiscardCommand>();

            // instance production
            MapCommandEndpoint<V1CreateItemProductionOrderCommand>();
            MapCommandEndpoint<V1CancelItemProductionOrderCommand>();
            MapCommandEndpoint<V1StartItemProductionOrderCommand>();
            MapCommandEndpoint<V1FinishItemProductionOrderCommand>();
        });
    }

    private static void MapCommandEndpoint<T>() where T : class
    {
        var name = typeof(T).Name.ToLower();
        EndpointConvention.Map<T>(new Uri($"queue:{name}"));
    }
}
