using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Instances.Discard;

public class V1InstanceDiscardCommandConsumer : BaseConsumer<V1InstanceDiscardCommand>
{
    public V1InstanceDiscardCommandConsumer(CreateInstanceDiscardedEvent.Handler handler)
        : base(handler.Handle) { }
}
