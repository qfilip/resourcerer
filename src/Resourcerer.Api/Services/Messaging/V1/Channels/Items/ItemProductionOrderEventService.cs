using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Application.Messaging.Channels;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;

namespace Resourcerer.Api.Services.Messaging.V1.Channels.Items;

public class ItemProductionOrderEventService : ChannelConsumerHostingService<V1ItemProductionCommand>
{
    public ItemProductionOrderEventService(
        IMessageReader<V1ItemProductionCommand> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) { }

    protected override Task HandleEvent(V1ItemProductionCommand message, AppDbContext appDbContext)
    {
        if (message is V1CreateCompositeItemProductionOrderCommand createComposite)
        {
            var handler = new CreateCompositeItemProductionOrder.Handler(appDbContext);
            return handler.Handle(createComposite);
        }
        else if (message is V1CancelCompositeItemProductionOrderCommand cancel)
        {
            var handler = new CancelCompositeItemProductionOrder.Handler(appDbContext);
            return handler.Handle(cancel);
        }
        else if (message is V1CreateElementItemProductionOrderCommand createElement)
        {
            var handler = new CreateElementItemProductionOrder.Handler(appDbContext);
            return handler.Handle(createElement);
        }
        else if (message is V1CancelElementItemProductionOrderCommand cancelElement)
        {
            var handler = new CancelElementItemProductionOrder.Handler(appDbContext);
            return handler.Handle(cancelElement);
        }
        else if (message is V1StartItemProductionOrderCommand start)
        {
            var handler = new StartItemProductionOrder.Handler(appDbContext);
            return handler.Handle(start);
        }
        else if (message is V1FinishItemProductionOrderCommand finish)
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
