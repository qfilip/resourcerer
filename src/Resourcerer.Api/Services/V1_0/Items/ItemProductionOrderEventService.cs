using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1_0.Commands.Items;

namespace Resourcerer.Api.Services.V1_0;

public class ItemProductionOrderEventService : EventConsumerServiceBase<ItemProductionEventBaseDto>
{
    public ItemProductionOrderEventService(
        IConsumerAdapter<ItemProductionEventBaseDto> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) {}

    protected override Task HandleEvent(ItemProductionEventBaseDto message, AppDbContext appDbContext)
    {
        if(message is CreateItemProductionOrderRequestDto create)
        {
            var handler = new CreateItemProductionOrder.Handler(appDbContext);
            return handler.Handle(create);
        }
        else if(message is CancelItemProductionOrderRequestDto cancel)
        {
            var handler = new CancelItemProductionOrder.Handler(appDbContext);
            return handler.Handle(cancel);
        }
        else if (message is StartItemProductionOrderRequestDto start)
        {
            var handler = new StartItemProductionOrder.Handler(appDbContext);
            return handler.Handle(start);
        }
        else
        {
            throw new InvalidOperationException("Unsupported event type");
        }
    }
}
