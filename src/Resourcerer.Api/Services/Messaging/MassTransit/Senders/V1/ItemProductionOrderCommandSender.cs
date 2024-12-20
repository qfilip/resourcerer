﻿using MassTransit;
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
        if (message is V1CreateCompositeItemProductionOrderCommand createComposite)
        {
            return _bus.Send(createComposite);
        }
        else if (message is V1CreateElementItemProductionOrderCommand createElement)
        {
            return _bus.Send(createElement);
        }
        else if (message is V1CancelCompositeItemProductionOrderCommand cancel)
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
