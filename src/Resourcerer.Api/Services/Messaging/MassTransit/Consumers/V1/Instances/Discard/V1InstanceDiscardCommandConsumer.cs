using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Instances.Discard;

public class V1InstanceDiscardCommandConsumer : ConsumerBase<V1InstanceDiscardCommand>
{
    public V1InstanceDiscardCommandConsumer(CreateInstanceDiscardedEvent.Handler handler)
        : base(handler.Handle) { }
}
