using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Items.Production;

public class V1CancelElementItemProductionOrderCommandConsumer : ConsumerBase<V1CancelElementItemProductionOrderCommand>
{
    public V1CancelElementItemProductionOrderCommandConsumer(CancelElementItemProductionOrder.Handler handler)
        : base(handler.Handle) { }
}
