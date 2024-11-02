using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers.Items.Production;

public class V1FinishItemProductionOrderCommandConsumer : BaseConsumer<V1FinishItemProductionOrderCommand>
{
    public V1FinishItemProductionOrderCommandConsumer(FinishItemProductionOrder.Handler handler)
        : base(handler.Handle) {}
}
