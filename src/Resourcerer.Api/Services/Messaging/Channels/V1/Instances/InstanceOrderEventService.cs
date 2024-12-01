using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1.Instances.Events.Order;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services.Messaging.Channels.V1.Instances;

public class InstanceOrderEventService : ChannelConsumerHostingServiceBase<V1InstanceOrderCommand>
{
    public InstanceOrderEventService(
        IMessageReader<V1InstanceOrderCommand> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) { }

    protected override Task HandleEvent(V1InstanceOrderCommand message, AppDbContext appDbContext)
    {
        Func<Task<HandlerResult<Unit>>> handler = message switch
        {
            V1InstanceOrderCreateCommand create =>
                () => new CreateInstanceOrderedEvent.Handler(appDbContext).Handle(create),

            V1InstanceOrderCancelCommand cancel =>
                () => new CreateInstanceOrderCancelledEvent.Handler(appDbContext).Handle(cancel),

            V1InstanceOrderDeliverCommand deliver =>
                () => new CreateInstanceOrderDeliveredEvent.Handler(appDbContext).Handle(deliver),

            V1InstanceOrderSendCommand send =>
                () => new CreateInstanceOrderSentEvent.Handler(appDbContext).Handle(send),

            _ => throw new InvalidOperationException("Unsupported event type")
        };

        return handler();
    }
}
