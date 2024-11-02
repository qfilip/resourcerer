using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Items.Production;

public class V1CreateCompositeItemProductionOrderCommandConsumer : BaseConsumer<V1CreateCompositeItemProductionOrderCommand>
{
    public V1CreateCompositeItemProductionOrderCommandConsumer(CreateCompositeItemProductionOrder.Handler handler)
        : base(handler.Handle) { }
}
