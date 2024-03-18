using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Instances.Events.Order;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.Logic.V1_0.Commands;
using System.Threading.Channels;

namespace Resourcerer.Api.Services.V1_0;

public class InstanceOrderEventService : EventServiceBase<InstanceOrderEventDtoBase>
{
    public InstanceOrderEventService(
        ChannelReader<InstanceOrderEventDtoBase> reader,
        IServiceProvider serviceProvider) : base(reader, serviceProvider) {}

    protected override Task HandleEvent(InstanceOrderEventDtoBase message, AppDbContext appDbContext)
    {
        if (message is InstanceOrderRequestDto orderEv)
        {
            var handler = new CreateInstanceOrderedEvent.Handler(appDbContext);
            return handler.Handle(orderEv);
        }
        else if (message is InstanceOrderCancelRequestDto cancelEv)
        {
            var handler = new CreateInstanceOrderCancelledEvent.Handler(appDbContext);
            return handler.Handle(cancelEv);
        }
        else if (message is InstanceOrderDeliveredRequestDto deliverEv)
        {
            var handler = new CreateInstanceOrderDeliveredEvent.Handler(appDbContext);
            return handler.Handle(deliverEv);
        }
        else if (message is InstanceOrderSentRequestDto sentEv)
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
