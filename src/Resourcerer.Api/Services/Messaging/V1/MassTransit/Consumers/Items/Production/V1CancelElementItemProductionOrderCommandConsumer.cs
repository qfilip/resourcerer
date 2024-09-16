﻿using Resourcerer.Application.Messaging.MassTransit;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Items.Production;

public class V1CancelElementItemProductionOrderCommandConsumer : BaseConsumer<V1CancelElementItemProductionOrderCommand>
{
    public V1CancelElementItemProductionOrderCommandConsumer(CancelElementItemProductionOrder.Handler handler)
        : base(handler.Handle) {}
}