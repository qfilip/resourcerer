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
        var task = message switch
        {
            V1InstanceOrderCreateCommand orderEv => _bus.Send(orderEv),
            V1InstanceOrderCancelCommand cancelEv => _bus.Send(cancelEv),
            V1InstanceOrderDeliverCommand deliverEv => _bus.Send(deliverEv),
            V1InstanceOrderSendCommand sentEv => _bus.Send(sentEv),
            _ => throw new InvalidOperationException("Unsupported event type")
        };
        
        return task;
    }
}
