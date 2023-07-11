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
                    .ThenInclude(x => x.InstanceDiscardedEvents)
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

            var pendingForStock = pendingInstances
                .Sum(x => x.UnitsOrdered);

            var deliveredInstances = item.Instances
                .Where(x =>
                    x.InstanceOrderedEvent != null &&
                    x.InstanceOrderedEvent.InstanceOrderDeliveredEvent != null)
                .ToArray();

            var unitsInStock = deliveredInstances
                .Select(x =>
                {
                    if (x.ExpiryDate <= DateTime.UtcNow) return 0;
                    
                    var discarded = x.InstanceDiscardedEvents.Sum(ev => ev.Quantity);
                    
                    return x.UnitsOrdered - discarded;
                });

            var isComposite = item.CompositeExcerpts.Any();

            var usedInComposites = item.ElementExcerpts
                .DistinctBy(x => x.CompositeId)
                .Count();

            return HandlerResult<List<ItemStatisticsDto>>.Ok(new List<ItemStatisticsDto>());
        }
    }
}
