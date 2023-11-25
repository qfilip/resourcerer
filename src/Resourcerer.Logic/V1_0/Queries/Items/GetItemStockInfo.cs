using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Items;

namespace Resourcerer.Logic.Queries.V1_0;

public static class GetItemStockInfo
{
    public class Handler : IAppHandler<(Guid ItemId, DateTime Now), ItemStockInfoDto>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<ItemStockInfoDto>> Handle((Guid ItemId, DateTime Now) request)
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
                    .ThenInclude(x => x.InstanceBoughtEvent)
                        .ThenInclude(x => x!.InstanceDeliveredEvent)
                // cancelations
                .Include(x => x.Instances)
                    .ThenInclude(x => x.InstanceBoughtEvent)
                        .ThenInclude(x => x!.InstanceCancelledEvent)
                // discards
                .Include(x => x.Instances)
                    .ThenInclude(x => x.InstanceDiscardedEvents)
                .FirstOrDefaultAsync(x => x.Id == request.ItemId);

            if (item == null)
            {
                return HandlerResult<ItemStockInfoDto>.NotFound($"Item with id {request.ItemId} doesn't exist");
            }

            var instancesInfo = item.Instances
                .Select(x => Functions.V1_0.Instances.GetInstanceInfo(x, request.Now))
                .OfType<InstanceInfoDto>()
                .ToArray();

            var isComposite = item.CompositeExcerpts.Any();

            var usedInComposites = item.ElementExcerpts.Count();

            var productionCostAsComposite = item.CompositeExcerpts
                .SelectMany(x => x.Element!.Prices)
                .Sum(x => x.UnitValue);

            var result = new ItemStockInfoDto
            {
                Id = item.Id,
                Name = item.Name,
                TotalUnitsInStock = instancesInfo.Sum(x => x.QuantityLeft),
                InstancesInfo = instancesInfo,
                ItemType = isComposite ? new[] { "Element", "Composite" } : new[] { "Element" },
                ProductionCostAsComposite = productionCostAsComposite,
                SellingPrice = item.Prices.Single().UnitValue
            };

            return HandlerResult<ItemStockInfoDto>.Ok(result);
        }
    }
}
