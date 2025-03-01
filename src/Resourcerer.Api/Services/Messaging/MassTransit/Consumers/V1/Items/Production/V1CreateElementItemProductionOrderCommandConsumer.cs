using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Items.Production;

public class V1CreateElementItemProductionOrderCommandConsumer : ConsumerBase<V1CreateElementItemProductionOrderCommand>
{
    public V1CreateElementItemProductionOrderCommandConsumer(CreateElementItemProductionOrder.Handler handler)
        : base(handler.Handle) { }
}
