﻿using Resourcerer.Application.Messaging.MassTransit;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Items.Production;

public class V1CreateElementItemProductionOrderCommandConsumer : BaseConsumer<V1CreateElementItemProductionOrderCommand>
{
    public V1CreateElementItemProductionOrderCommandConsumer(CreateElementItemProductionOrder.Handler handler)
        : base(handler.Handle) {}
}
