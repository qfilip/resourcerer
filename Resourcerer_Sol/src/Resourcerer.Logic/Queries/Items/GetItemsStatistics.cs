using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Elements;

namespace Resourcerer.Logic.Queries.Items;
public static class GetItemsStatistics
{
    public class Handler : IAppHandler<Unit, List<ItemStatisticsDto>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<List<ItemStatisticsDto>>> Handle(Unit _)
        {
            var item = await _appDbContext.Items
                .Include(x => x.Category)
                .Include(x => x.UnitOfMeasure)
                .Include(x => x.Prices)
                .Include(x => x.ElementExcerpts)
                .Include(x => x.CompositeExcerpts)
                // orders
                .Include(x => x.Instances)
                    .ThenInclude(x => x.InstanceOrderedEvent)
                        .ThenInclude(x => x!.InstanceOrderDeliveredEvent)
                // cancelations
                .Include(x => x.Instances)
                    .ThenInclude(x => x.InstanceOrderedEvent)
                        .ThenInclude(x => x!.InstanceOrderCancelledEvent)
                // discards
                .Include(x => x.Instances)
                    .ThenInclude(x => x.InstanceDiscardedEvent)
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return HandlerResult<List<ItemStatisticsDto>>.Ok(new List<ItemStatisticsDto>());
            }

            var pendingInstances = item.Instances
                .Where(x =>
                    x.InstanceOrderedEvent != null &&
                    x.InstanceOrderedEvent.InstanceOrderCancelledEvent == null &&
                    x.InstanceOrderedEvent.InstanceOrderDeliveredEvent == null)
                .ToArray();

            var cancelledInstances = item.Instances
                .Where(x =>
                    x.InstanceOrderedEvent != null &&
                    x.InstanceOrderedEvent.InstanceOrderCancelledEvent != null)
                .ToArray();

            var deliveredInstances = item.Instances
                .Where(x =>
                    x.InstanceOrderedEvent != null &&
                    x.InstanceOrderedEvent.InstanceOrderDeliveredEvent != null)
                .ToArray();

            var unitsInStock = deliveredInstances
                .Select(x =>
                {
                    var totalQuantity = x.UnitsOrdered;
                    if(x.InstanceDiscardedEvent != null)
                    {
                        totalQuantity -= x.InstanceDiscardedEvent.Quantity;
                    }

                    return totalQuantity;
                });


            return HandlerResult<List<ItemStatisticsDto>>.Ok(new List<ItemStatisticsDto>());
        }
    }
}
