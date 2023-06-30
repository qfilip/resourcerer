using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Elements;

namespace Resourcerer.Logic.Queries.Elements;
public static class GetAllElementsOverview
{
    public class Handler : IRequestHandler<Unit, List<ElementUsageDetailsDto>>
    {
        private readonly IAppDbContext _appDbContext;

        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<List<ElementUsageDetailsDto>>> Handle(Unit _)
        {
            var elementsData = await _appDbContext.Elements
                .IgnoreQueryFilters()
                .Include(x => x.ElementSoldEvents)
                .Include(x => x.ElementPurchasedEvents)
                .Include(x => x.UnitOfMeasure)
                .IgnoreQueryFilters()
                .Include(x => x.Excerpts)
                .IgnoreQueryFilters()
                .ToListAsync();

            var idLookup = elementsData
                .Select(x => new
                {
                    ElementId = x.Id,
                    ElementCompositeIds = x.Excerpts
                        .Select(e => e.CompositeId)
                        .ToList()
                })
                .ToList();

            var compositeIds = idLookup
                .SelectMany(x => x.ElementCompositeIds)
                .ToList();

            var compositeSoldEvents = await _appDbContext.CompositeSoldEvents
                .Where(x => compositeIds.Contains(x.CompositeId))
                .ToListAsync();

            var compositeSoldEventsCompositeIds = compositeSoldEvents.Select(cse => cse.CompositeId).ToList();

            var usageDetails = elementsData.Select(x =>
            {
                var unitsPurchased = x.ElementPurchasedEvents
                    .Sum(epe => epe.UnitsBought);

                var purchaseCosts = x.ElementPurchasedEvents
                    .Sum(epe => epe.PriceByUnit * epe.UnitsBought);

                var elementCompositeIds = idLookup
                    .Where(il => il.ElementId == x.Id)
                    .SelectMany(i => i.ElementCompositeIds)
                    .ToList();

                var unitsSoldRaw = x.ElementSoldEvents.Sum(ese => ese.UnitsSold);

                var relevantExcerptsLookup = x.Excerpts
                    .Where(e => compositeSoldEventsCompositeIds.Contains(e.CompositeId))
                    .Select(e => new
                    {
                        e.CompositeId,
                        ElementQuantity = e.Quantity
                    })
                    .ToList();

                var unitsUsedInComposites = compositeSoldEvents.Aggregate(0d, (acc, cse) =>
                {
                    var quantityInfo = relevantExcerptsLookup
                        .FirstOrDefault(rel => rel.CompositeId == cse.CompositeId);

                    if (quantityInfo != null)
                    {
                        acc += quantityInfo.ElementQuantity;
                    }

                    return acc;
                });

                return new ElementUsageDetailsDto
                {
                    ElementId = x.Id,
                    ElementName = x.Name,
                    Unit = x.UnitOfMeasure!.Name,
                    UnitsPurchased = unitsPurchased,
                    PurchaseCosts = purchaseCosts,
                    UnitsUsed = unitsUsedInComposites,
                    UnitsInStock = unitsPurchased - unitsUsedInComposites - unitsSoldRaw
                };
            })
            .ToList();

            return HandlerResult<List<ElementUsageDetailsDto>>.Ok(usageDetails);
        }
    }
}
