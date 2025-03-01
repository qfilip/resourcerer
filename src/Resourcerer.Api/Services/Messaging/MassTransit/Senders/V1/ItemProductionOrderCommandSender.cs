using MassTransit;
using Resourcerer.Dtos.V1;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Senders.V1;

public class ItemProductionOrderCommandSender : IMessageSender<V1ItemProductionCommand>
{
    private readonly IBus _bus;
    public ItemProductionOrderCommandSender(IBus bus)
    {
        _bus = bus;
    }

    public Task SendAsync(V1ItemProductionCommand message)
    {
        var task = message switch
        {
            V1CreateCompositeItemProductionOrderCommand createComposite => _bus.Send(createComposite),
            V1CreateElementItemProductionOrderCommand createElement => _bus.Send(createElement),
            V1CancelCompositeItemProductionOrderCommand cancel => _bus.Send(cancel),
            V1StartItemProductionOrderCommand start => _bus.Send(start),
            V1FinishItemProductionOrderCommand finish => _bus.Send(finish),
            _ => throw new InvalidOperationException("Unsupported event type")
        };
        
        return task;
    }
}
