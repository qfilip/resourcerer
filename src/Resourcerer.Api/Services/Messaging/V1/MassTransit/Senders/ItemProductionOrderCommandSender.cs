using MassTransit;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Senders;

public class ItemProductionOrderCommandSender : IMessageSender<V1ItemProductionCommand>
{
    private readonly IBus _bus;
    public ItemProductionOrderCommandSender(IBus bus)
    {
        _bus = bus;
    }

    public Task SendAsync(V1ItemProductionCommand message)
    {
        if (message is V1CreateCompositeItemProductionOrderCommand create)
        {
            return _bus.Send(create);
        }
        else if (message is V1CancelItemProductionOrderCommand cancel)
        {
            return _bus.Send(cancel);
        }
        else if (message is V1StartItemProductionOrderCommand start)
        {
            return _bus.Send(start);
        }
        else if (message is V1FinishItemProductionOrderCommand finish)
        {
            return _bus.Send(finish);
        }
        else
        {
            throw new InvalidOperationException("Unsupported event type");
        }
    }
}
