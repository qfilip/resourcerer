using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Items.Production;

public class V1CancelCompositeItemProductionOrderCommandConsumer : BaseConsumer<V1CancelCompositeItemProductionOrderCommand>
{
    public V1CancelCompositeItemProductionOrderCommandConsumer(CancelCompositeItemProductionOrder.Handler handler)
        : base(handler.Handle) {}
}
