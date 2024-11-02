using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Items.Production;

public class V1FinishItemProductionOrderCommandConsumer : BaseConsumer<V1FinishItemProductionOrderCommand>
{
    public V1FinishItemProductionOrderCommandConsumer(FinishItemProductionOrder.Handler handler)
        : base(handler.Handle) { }
}
