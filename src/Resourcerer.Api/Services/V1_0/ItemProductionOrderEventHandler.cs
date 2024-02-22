using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using System.Threading.Channels;

namespace Resourcerer.Api.Services.V1_0;

public class ItemProductionOrderEventHandler : EventServiceBase<ItemProductionEventBaseDto>
{
    public ItemProductionOrderEventHandler(ChannelReader<ItemProductionEventBaseDto> reader, IServiceProvider serviceProvider) : base(reader, serviceProvider)
    {
    }

    protected override Task HandleEvent(ItemProductionEventBaseDto message, AppDbContext appDbContext)
    {
        throw new NotImplementedException();
    }
}
