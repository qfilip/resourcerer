using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Elements;

namespace Resourcerer.Logic.Elements.Queries;
public static class GetAllElementsOverview
{
    public class Handler : IRequestHandler<Unit, List<ElementUsageDetailsDto>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<List<ElementUsageDetailsDto>>> Handle(Unit _)
        {
            var elementsData = await _appDbContext.Elements
                .Include(x => x.ElementSoldEvents)
                .Include(x => x.ElementPurchasedEvents)
                .Include(x => x.UnitOfMeasure)
                .Include(x => x.Category)
                .Include(x => x.Excerpts)
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
                .Include(x => x.Price)
                .ToListAsync();

            var usageDetails = elementsData.Select(x => {
                var unitsPurchased = x.ElementPurchasedEvents
                    .Sum(epe => epe.NumOfUnits);

                var purchaseCosts = x.ElementPurchasedEvents
                    .Sum(epe => epe.UnitPrice);

                var elementCompositeIds = idLookup
                    .Where(il => il.ElementId == x.Id)
                    .SelectMany(i => i.ElementCompositeIds)
                    .ToList();

                var unitsSoldRaw = x.ElementSoldEvents.Count;

                var unitsUsedInComposites = x.Excerpts
                    .Where(e => compositeSoldEvents.Select(cse => cse.CompositeId).Contains(e.CompositeId))
                    .Sum(e => e.Quantity);
                
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
