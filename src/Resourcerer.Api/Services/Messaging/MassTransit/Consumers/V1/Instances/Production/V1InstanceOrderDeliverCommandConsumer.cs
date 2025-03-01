using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Instances.Production;

public class V1InstanceOrderDeliverCommandConsumer : ConsumerBase<V1InstanceOrderDeliverCommand>
{
    public V1InstanceOrderDeliverCommandConsumer(CreateInstanceOrderDeliveredEvent.Handler handler)
        : base(handler.Handle) { }
}
