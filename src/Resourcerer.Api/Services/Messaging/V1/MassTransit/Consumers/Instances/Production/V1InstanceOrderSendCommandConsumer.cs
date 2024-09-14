using Resourcerer.Application.Messaging.MassTransit;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Instances.Production;

public class V1InstanceOrderSendCommandConsumer : BaseConsumer<V1InstanceOrderSendCommand>
{
    public V1InstanceOrderSendCommandConsumer(CreateInstanceOrderSentEvent.Handler handler)
        : base(handler.Handle) { }
}
