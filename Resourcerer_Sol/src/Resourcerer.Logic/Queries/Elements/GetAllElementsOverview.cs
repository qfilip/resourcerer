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

            var compositeSoldEventsLookup = compositeSoldEvents
                .Select(cse => new
                {
                    CompositeId = cse.CompositeId,
                    UnitsSold = cse.UnitsSold

                }).ToLookup(x => x.CompositeId);

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

                var unitsUsedInComposites = x.Excerpts
                    .Select(e => {
                        var compositeUnitsSold = compositeSoldEventsLookup[e.CompositeId].Sum(x => x.UnitsSold);
                        return new
                        {
                            e.CompositeId,
                            ElementQuantity = e.Quantity * compositeUnitsSold
                        };
                    })
                    .Sum(x => x.ElementQuantity);

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
