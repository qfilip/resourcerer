using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;
using Resourcerer.Messaging.Abstractions;
using Resourcerer.Messaging.Channels;

namespace Resourcerer.Api.Services.V1;

public class InstanceOrderEventService : ChannelConsumerHostingService<V1InstanceOrderEvent>
{
    public InstanceOrderEventService(
        IMessageConsumer<V1InstanceOrderEvent> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) {}

    protected override Task HandleEvent(V1InstanceOrderEvent message, AppDbContext appDbContext)
    {
        if (message is V1InstanceOrderRequest orderEv)
        {
            var handler = new CreateInstanceOrderedEvent.Handler(appDbContext);
            return handler.Handle(orderEv);
        }
        else if (message is V1InstanceOrderCancelRequest cancelEv)
        {
            var handler = new CreateInstanceOrderCancelledEvent.Handler(appDbContext);
            return handler.Handle(cancelEv);
        }
        else if (message is V1InstanceOrderDeliveredRequest deliverEv)
        {
            var handler = new CreateInstanceOrderDeliveredEvent.Handler(appDbContext);
            return handler.Handle(deliverEv);
        }
        else if (message is V1InstanceOrderSentRequest sentEv)
        {
            var handler = new CreateInstanceOrderSentEvent.Handler(appDbContext);
            return handler.Handle(sentEv);
        }
        else
        {
            throw new InvalidOperationException("Unsupported event type");
        }
    }
}
