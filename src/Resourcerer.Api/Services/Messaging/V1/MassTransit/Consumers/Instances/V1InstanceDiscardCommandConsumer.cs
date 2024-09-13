using MassTransit;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Instances;

public class V1InstanceDiscardCommandConsumer : BaseConsumer<V1InstanceDiscardCommand>
{
    public V1InstanceDiscardCommandConsumer(CreateInstanceDiscardedEvent.Handler handler)
        : base(handler.Handle) { }
}
