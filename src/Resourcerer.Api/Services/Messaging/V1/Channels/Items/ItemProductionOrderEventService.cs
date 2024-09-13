using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Application.Messaging.Channels;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;

namespace Resourcerer.Api.Services.Messaging.V1.Channels.Items;

public class ItemProductionOrderEventService : ChannelConsumerHostingService<V1ItemProductionCommand>
{
    public ItemProductionOrderEventService(
        IMessageConsumer<V1ItemProductionCommand> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) { }

    protected override Task HandleEvent(V1ItemProductionCommand message, AppDbContext appDbContext)
    {
        if (message is V1CreateItemProductionOrderCommand create)
        {
            var handler = new CreateItemProductionOrder.Handler(appDbContext);
            return handler.Handle(create);
        }
        else if (message is V1CancelItemProductionOrderCommand cancel)
        {
            var handler = new CancelItemProductionOrder.Handler(appDbContext);
            return handler.Handle(cancel);
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
