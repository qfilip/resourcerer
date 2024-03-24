using Resourcerer.Api.Services.Messaging;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Commands.Items;

namespace Resourcerer.Api.Services.V1;

public class ItemProductionOrderEventService : EventConsumerServiceBase<V1ItemProductionEvent>
{
    public ItemProductionOrderEventService(
        IConsumerAdapter<V1ItemProductionEvent> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) {}

    protected override Task HandleEvent(V1ItemProductionEvent message, AppDbContext appDbContext)
    {
        if(message is V1CreateItemProductionOrderRequest create)
        {
            var handler = new CreateItemProductionOrder.Handler(appDbContext);
            return handler.Handle(create);
        }
        else if(message is V1CancelItemProductionOrderRequest cancel)
        {
            var handler = new CancelItemProductionOrder.Handler(appDbContext);
            return handler.Handle(cancel);
        }
        else if (message is V1StartItemProductionOrderRequest start)
        {
            var handler = new StartItemProductionOrder.Handler(appDbContext);
            return handler.Handle(start);
        }
        else if (message is V1FinishItemProductionOrderRequest finish)
        {
            var handler = new FinishItemProductionOrder.Handler(appDbContext);
            return handler.Handle(finish);
        }
        else
        {
            throw new InvalidOperationException("Unsupported event type");
        }
    }
}
