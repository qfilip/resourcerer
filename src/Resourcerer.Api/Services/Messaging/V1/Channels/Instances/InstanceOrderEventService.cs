using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Application.Messaging.Channels;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;

namespace Resourcerer.Api.Services.Messaging.V1.Channels.Instances;

public class InstanceOrderEventService : ChannelConsumerHostingService<V1InstanceOrderCommand>
{
    public InstanceOrderEventService(
        IMessageReader<V1InstanceOrderCommand> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) { }

    protected override Task HandleEvent(V1InstanceOrderCommand message, AppDbContext appDbContext)
    {
        if (message is V1InstanceOrderCreateCommand orderEv)
        {
            var handler = new V1CreateInstanceOrderedEvent.Handler(appDbContext);
            return handler.Handle(orderEv);
        }
        else if (message is V1InstanceOrderCancelCommand cancelEv)
        {
            var handler = new CreateInstanceOrderCancelledEvent.Handler(appDbContext);
            return handler.Handle(cancelEv);
        }
        else if (message is V1InstanceOrderDeliverCommand deliverEv)
        {
            var handler = new CreateInstanceOrderDeliveredEvent.Handler(appDbContext);
            return handler.Handle(deliverEv);
        }
        else if (message is V1InstanceOrderSendCommand sentEv)
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
