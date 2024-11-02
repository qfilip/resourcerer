﻿using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Instances.Production;

public class V1InstanceOrderDeliverCommandConsumer : BaseConsumer<V1InstanceOrderDeliverCommand>
{
    public V1InstanceOrderDeliverCommandConsumer(CreateInstanceOrderDeliveredEvent.Handler handler)
        : base(handler.Handle) { }
}
