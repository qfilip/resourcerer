using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Items.Production;

public class V1CancelCompositeItemProductionOrderCommandConsumer : ConsumerBase<V1CancelCompositeItemProductionOrderCommand>
{
    public V1CancelCompositeItemProductionOrderCommandConsumer(CancelCompositeItemProductionOrder.Handler handler)
        : base(handler.Handle) { }
}
