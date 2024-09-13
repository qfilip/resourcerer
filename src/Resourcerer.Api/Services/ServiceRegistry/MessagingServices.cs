using MassTransit;
using Resourcerer.Api.Services.Messaging.V1.Channels.Instances;
using Resourcerer.Api.Services.Messaging.V1.Channels.Items;
using Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Instances;
using Resourcerer.Application.Messaging;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddChannelMessagingServices(IServiceCollection services, bool useChannels)
    {
        if(useChannels)
        {
            DependencyInjection.AddChannelMessagingService<V1InstanceOrderCommand, InstanceOrderEventService>(services);
            DependencyInjection.AddChannelMessagingService<V1InstanceDiscardCommand, InstanceDiscardEventService>(services);
            DependencyInjection.AddChannelMessagingService<V1ItemProductionCommand, ItemProductionOrderEventService>(services);
            
            return;
        }

        services.AddMassTransit(c =>
        {
            // instance discard
            c.AddConsumer<V1InstanceDiscardCommandConsumer>();

            // instance order
            c.AddConsumer<V1InstanceOrderCreateCommandConsumer>();
            c.AddConsumer<V1InstanceOrderCancelCommandConsumer>();
            c.AddConsumer<V1InstanceOrderDeliverCommandConsumer>();
            c.AddConsumer<V1InstanceOrderSendCommandConsumer>();

            c.UsingInMemory((ctx, cfg) =>
            {
                cfg.ReceiveEndpoint("", q =>
                {
                    // instance discard
                    q.ConfigureConsumer<V1InstanceDiscardCommandConsumer>(ctx);

                    // instance order
                    q.ConfigureConsumer<V1InstanceOrderCreateCommandConsumer>(ctx);
                    q.ConfigureConsumer<V1InstanceOrderCancelCommandConsumer>(ctx);
                    q.ConfigureConsumer<V1InstanceOrderDeliverCommandConsumer>(ctx);
                    q.ConfigureConsumer<V1InstanceOrderSendCommandConsumer>(ctx);
                });

                // instance discard
                cfg.Send<V1InstanceDiscardCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));

                // instance order
                cfg.Send<V1InstanceOrderCreateCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
                cfg.Send<V1InstanceOrderCancelCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
                cfg.Send<V1InstanceOrderDeliverCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
                cfg.Send<V1InstanceOrderSendCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));

                // item production
                cfg.Send<V1CreateItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ItemId));
                cfg.Send<V1CancelItemProductionOrderCommand>(cmd => cmd.UseCorrelationId(x => x.ProductionOrderId));
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
