using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos.Elements;

namespace Resourcerer.Logic.Queries.Items;
public static class GetItemStatistics
{
    public class Handler : IAppHandler<(Guid ItemId, DateTime Now), List<ItemStatisticsDto>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<List<ItemStatisticsDto>>> Handle((Guid ItemId, DateTime Now) query)
        {
            var item = await _appDbContext.Items
                .Include(x => x.Category)
                .Include(x => x.UnitOfMeasure)
                .Include(x => x.Prices)
                .Include(x => x.ElementExcerpts)
                .Include(x => x.CompositeExcerpts)
                    .ThenInclude(x => x.Element)
                        .ThenInclude(x => x!.Prices)
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
                .FirstOrDefaultAsync(x => x.Id == query.ItemId);

            if (item == null)
            {
                return HandlerResult<List<ItemStatisticsDto>>.Ok(new List<ItemStatisticsDto>());
            }

            var pendingBoughtInstances = item.Instances
                .Where(x =>
                    x.InstanceOrderedEvent != null &&
                    x.InstanceOrderedEvent.OrderType == eOrderType.Buy &&
                    x.InstanceOrderedEvent.InstanceOrderCancelledEvent == null &&
                    x.InstanceOrderedEvent.InstanceOrderDeliveredEvent == null)
                .ToArray();

            var pendingForStock = pendingBoughtInstances
                .Sum(x => x.UnitsOrdered);

            var deliveredBoughtInstances = item.Instances
                .Where(x =>
                    x.InstanceOrderedEvent != null &&
                    x.InstanceOrderedEvent.OrderType == eOrderType.Buy &&
                    x.InstanceOrderedEvent.InstanceOrderDeliveredEvent != null)
                .ToArray();
            
            var totalUnitsInStock = deliveredBoughtInstances
                .Select(x =>
                {
                    if (x.ExpiryDate <= query.Now) return 0;
                    
                    var discarded = x.InstanceDiscardedEvents.Sum(ev => ev.Quantity);
                    
                    return x.UnitsOrdered - discarded;
                }).Sum();

            var isComposite = item.CompositeExcerpts.Any();

            var usedInComposites = item.ElementExcerpts.Count();

            var makingCostAsComposite = item.CompositeExcerpts
                .SelectMany(x => x.Element!.Prices)
                .Sum(x => x.UnitValue);

            var sellingCost = item.Prices.Single().UnitValue;

            return HandlerResult<List<ItemStatisticsDto>>.Ok(new List<ItemStatisticsDto>());
        }
    }
}
