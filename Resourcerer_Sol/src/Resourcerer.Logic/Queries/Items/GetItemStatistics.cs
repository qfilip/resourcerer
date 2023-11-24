using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
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
                // delivered
                .Include(x => x.Instances)
                    .ThenInclude(x => x.InstanceBuyRequestedEvent)
                        .ThenInclude(x => x!.InstanceDeliveredEvent)
                // sells
                .Include(x => x.Instances)
                    .ThenInclude(x => x.InstanceSellRequestedEvents)
                        .ThenInclude(x => x!.InstanceCancelledEvent)
                // cancelations
                .Include(x => x.Instances)
                    .ThenInclude(x => x.InstanceBuyRequestedEvent)
                        .ThenInclude(x => x!.InstanceCancelledEvent)
                // discards
                .Include(x => x.Instances)
                    .ThenInclude(x => x.InstanceDiscardedEvents)
                .FirstOrDefaultAsync(x => x.Id == query.ItemId);

            if (item == null)
            {
                return HandlerResult<List<ItemStatisticsDto>>.Ok(new List<ItemStatisticsDto>());
            }

            var instanceInfos = item.Instances
                .Select(x => Functions.Instances.GetInstanceInfo(x, query.Now))
                .ToArray();

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
