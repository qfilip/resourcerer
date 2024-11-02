using MassTransit;
using Resourcerer.Dtos.V1;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Senders.V1;

public class InstanceOrderCommandSender : IMessageSender<V1InstanceOrderCommand>
{
    private readonly IBus _bus;
    public InstanceOrderCommandSender(IBus bus)
    {
        _bus = bus;
    }
    public Task SendAsync(V1InstanceOrderCommand message)
    {
        if (message is V1InstanceOrderCreateCommand orderEv)
        {
            return _bus.Send(orderEv);
        }
        else if (message is V1InstanceOrderCancelCommand cancelEv)
        {
            return _bus.Send(cancelEv);
        }
        else if (message is V1InstanceOrderDeliverCommand deliverEv)
        {
            return _bus.Send(deliverEv);
        }
        else if (message is V1InstanceOrderSendCommand sentEv)
        {
            return _bus.Send(sentEv);
        }
        else
        {
            throw new InvalidOperationException("Unsupported event type");
        }
    }
}
