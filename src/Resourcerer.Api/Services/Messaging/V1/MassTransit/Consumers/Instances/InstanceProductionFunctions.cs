using MassTransit;
using Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Instances.Production;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Instances;

public class InstanceProductionFunctions
{
    public static (
        Action<IBusRegistrationConfigurator> AddConsumers,
        Action<IInMemoryReceiveEndpointConfigurator, IBusRegistrationContext> ConfigureConsumers,
        Action<IInMemoryBusFactoryConfigurator> ConfigureCommands,
        Action MapCommandEndpoints) Get()
    {
        var addConsumers = (IBusRegistrationConfigurator x) =>
        {
            x.AddConsumer<V1InstanceOrderCreateCommandConsumer>();
            x.AddConsumer<V1InstanceOrderCancelCommandConsumer>();
            x.AddConsumer<V1InstanceOrderDeliverCommandConsumer>();
            x.AddConsumer<V1InstanceOrderSendCommandConsumer>();
        };

        var configureConsumers = (IInMemoryReceiveEndpointConfigurator q, IBusRegistrationContext ctx) =>
        {
            q.ConfigureConsumer<V1InstanceOrderCreateCommandConsumer>(ctx);
            q.ConfigureConsumer<V1InstanceOrderCancelCommandConsumer>(ctx);
            q.ConfigureConsumer<V1InstanceOrderDeliverCommandConsumer>(ctx);
            q.ConfigureConsumer<V1InstanceOrderSendCommandConsumer>(ctx);
        };

        var configureCommands = (IInMemoryBusFactoryConfigurator cfg) =>
        {
            cfg.Send<V1InstanceOrderCreateCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
            cfg.Send<V1InstanceOrderCancelCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
            cfg.Send<V1InstanceOrderDeliverCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
            cfg.Send<V1InstanceOrderSendCommand>(cmd => cmd.UseCorrelationId(x => x.InstanceId));
        };

        var mapCommandEndpoints = () =>
        {
            ServiceRegistry.MapCommandEndpoint<V1InstanceOrderCreateCommand>();
            ServiceRegistry.MapCommandEndpoint<V1InstanceOrderCancelCommand>();
            ServiceRegistry.MapCommandEndpoint<V1InstanceOrderDeliverCommand>();
            ServiceRegistry.MapCommandEndpoint<V1InstanceOrderSendCommand>();
        };


        return (addConsumers, configureConsumers, configureCommands, mapCommandEndpoints);
    }
}
