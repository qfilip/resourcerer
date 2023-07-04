using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Elements;
using Resourcerer.Utilities;

namespace Resourcerer.Logic.Queries.Elements;
public static class GetAllElementsStatistics
{
    public class Handler : IRequestHandler<Unit, List<ElementStatisticsDto>>
    {
        private readonly IAppDbContext _appDbContext;

        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<List<ElementStatisticsDto>>> Handle(Unit _)
        {
            //var elementsSales = await _appDbContext.ElementSoldEvents.ToListAsync();
            //var elementsPurchases = await _appDbContext.ElementPurchasedEvents.ToListAsync();

            //var elementsData = await _appDbContext.Elements
            //    .IgnoreQueryFilters()
            //    .Include(x => x.UnitOfMeasure)
            //    .IgnoreQueryFilters()
            //    .Include(x => x.Excerpts)
            //    .IgnoreQueryFilters()
            //    .ToListAsync();


            //var idLookup = elementsData
            //    .Select(x => new
            //    {
            //        ElementId = x.Id,
            //        ElementCompositeIds = x.Excerpts
            //            .Select(e => e.CompositeId)
            //            .ToList()
            //    })
            //    .ToList();

            //var compositeIds = idLookup
            //    .SelectMany(x => x.ElementCompositeIds)
            //    .ToList();

            //var compositeSoldEvents = await _appDbContext.CompositeSoldEvents
            //    .Where(x => compositeIds.Contains(x.CompositeId))
            //    .ToListAsync();

            //var compositeSoldEventsLookup = compositeSoldEvents
            //    .Select(cse => new
            //    {
            //        CompositeId = cse.CompositeId,
            //        UnitsSold = cse.UnitsSold

            //    }).ToLookup(x => x.CompositeId);

            //var usageDetails = elementsData.Select(x =>
            //{
            //    var events = new
            //    {
            //        Purchases = elementsPurchases
            //            .Where(e => e.ElementId == x.Id)
            //            .ToArray(),

            //        Sales = elementsSales
            //            .Where(e => e.ElementId == x.Id)
            //            .ToArray()
            //    };

            //    var unitsPurchased = events.Purchases
            //        .Sum(epe => epe.UnitsBought);

            //    var purchaseCosts = events.Purchases
            //        .Sum(epe => Maths.Discount(epe.UnitPrice * epe.UnitsBought, epe.TotalDiscountPercent));

            //    var averagePurchaseDiscount =
            //        Maths.SafeAverage(events.Purchases, x => x.TotalDiscountPercent);

            //    var elementCompositeIds = idLookup
            //        .Where(il => il.ElementId == x.Id)
            //        .SelectMany(i => i.ElementCompositeIds)
            //        .ToList();

            //    var unitsSoldRaw = events.Sales.Sum(ese => ese.UnitsSold);

            //    var salesEarnings = events.Sales
            //    .Sum(ese => Maths.Discount(ese.UnitsSold * ese.UnitPrice, ese.TotalDiscountPercent));

            //    var averageSaleDiscount = Maths.SafeAverage(events.Sales, e => e.TotalDiscountPercent);

            //    var unitsUsedInComposites = x.Excerpts
            //        .Select(e => {
            //            var compositeUnitsSold = compositeSoldEventsLookup[e.CompositeId].Sum(x => x.UnitsSold);
            //            return new
            //            {
            //                e.CompositeId,
            //                ElementQuantity = e.Quantity * compositeUnitsSold
            //            };
            //        })
            //        .Sum(x => x.ElementQuantity);

            //    var usedInComposites = x.Excerpts.DistinctBy(e => e.CompositeId).Count();

            //    return new ElementStatisticsDto
            //    {
            //        ElementId = x.Id,
            //        Name = x.Name,
            //        Unit = x.UnitOfMeasure!.Name,
            //        UnitsPurchased = unitsPurchased,
            //        PurchaseCosts = purchaseCosts,
            //        AveragePurchaseDiscount = averagePurchaseDiscount,
            //        UnitsSold = unitsSoldRaw,
            //        SalesEarning = salesEarnings,
            //        AverageSaleDiscount = averageSaleDiscount,
            //        UnitsUsedInComposites = unitsUsedInComposites,
            //        UsedInComposites = usedInComposites,
            //        UnitsInStock = unitsPurchased - unitsUsedInComposites - unitsSoldRaw
            //    };
            //})
            //.ToList();

            // return HandlerResult<List<ElementStatisticsDto>>.Ok(usageDetails);
            return HandlerResult<List<ElementStatisticsDto>>.Ok(new List<ElementStatisticsDto>());
        }
    }
}
