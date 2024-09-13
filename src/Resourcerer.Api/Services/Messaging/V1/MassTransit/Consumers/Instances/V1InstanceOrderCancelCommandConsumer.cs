using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Instances;

public class V1InstanceOrderCancelCommandConsumer : BaseConsumer<V1InstanceOrderCancelCommand>
{
    public V1InstanceOrderCancelCommandConsumer(CreateInstanceOrderCancelledEvent.Handler handler)
        : base(handler.Handle) { }
}
