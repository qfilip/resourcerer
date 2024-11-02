using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Instances.Production;

public class V1InstanceOrderCreateCommandConsumer : BaseConsumer<V1InstanceOrderCreateCommand>
{
    public V1InstanceOrderCreateCommandConsumer(CreateInstanceOrderedEvent.Handler handler)
        : base(handler.Handle) { }
}
