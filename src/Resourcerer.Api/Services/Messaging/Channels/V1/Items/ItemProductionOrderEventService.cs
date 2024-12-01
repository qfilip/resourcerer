using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services.Messaging.Channels.V1.Items;

public class ItemProductionOrderEventService : ChannelConsumerHostingServiceBase<V1ItemProductionCommand>
{
    public ItemProductionOrderEventService(
        IMessageReader<V1ItemProductionCommand> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) { }

    protected override Task HandleEvent(V1ItemProductionCommand message, AppDbContext appDbContext)
    {
        Func<Task<HandlerResult<Unit>>> handler = message switch
        {
            V1CreateCompositeItemProductionOrderCommand createComposite =>
                () => new CreateCompositeItemProductionOrder.Handler(appDbContext).Handle(createComposite),
            
            V1CancelCompositeItemProductionOrderCommand cancelComposite =>
                () => new CancelCompositeItemProductionOrder.Handler(appDbContext).Handle(cancelComposite),
            
            V1CreateElementItemProductionOrderCommand createElement =>
                () => new CreateElementItemProductionOrder.Handler(appDbContext).Handle(createElement),
            
            V1CancelElementItemProductionOrderCommand cancelElement =>
                () => new CancelElementItemProductionOrder.Handler(appDbContext).Handle(cancelElement),
            
            V1StartItemProductionOrderCommand start =>
                () => new StartItemProductionOrder.Handler(appDbContext).Handle(start),
            
            V1FinishItemProductionOrderCommand finish =>
                () => new FinishItemProductionOrder.Handler(appDbContext).Handle(finish),
            
            _ => throw new InvalidOperationException("Unsupported event type")
        };

        return handler();
    }
}
