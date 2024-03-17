using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1_0.Commands.Items;
using System.Threading.Channels;

namespace Resourcerer.Api.Services.V1_0;

public class ItemProductionOrderEventHandler : EventServiceBase<ItemProductionEventBaseDto>
{
    public ItemProductionOrderEventHandler(
        ChannelReader<ItemProductionEventBaseDto>
        reader, IServiceProvider serviceProvider) : base(reader, serviceProvider) {}

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
        else
        {
            throw new InvalidOperationException("Unsupported event type");
        }
    }
}
