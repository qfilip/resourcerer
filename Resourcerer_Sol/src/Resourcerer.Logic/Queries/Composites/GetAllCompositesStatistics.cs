using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Utilities;

namespace Resourcerer.Logic.Queries.Composites;

public static class GetAllCompositesStatistics
{
    public class Handler : IRequestHandler<Unit, List<CompositeStatisticsDto>>
    {
        private readonly IAppDbContext _appDbContext;
        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<List<CompositeStatisticsDto>>> Handle(Unit request)
        {
            var composites = await _appDbContext.Composites
                .Include(x => x.Prices)
                .Include(x => x.Excerpts)
                    .ThenInclude(x => x.Element)
                        .ThenInclude(x => x!.Prices)
                .ToArrayAsync();

            var compositeSoldEvents = await _appDbContext.CompositeSoldEvents
                .ToArrayAsync();

            var result = composites.Select(c =>
            {
                var soldEvents = compositeSoldEvents
                    .Where(cse => cse.CompositeId == c.Id)
                    .ToArray();

                var makingCosts = c.Excerpts
                    .Sum(x => x.Element!.Prices.Single().UnitValue * x.Quantity);

                return new CompositeStatisticsDto
                {
                    CompositeId = c.Id,
                    Name = c.Name,
                    UnitsSold = soldEvents.Sum(x => x.UnitsSold),
                    SaleEarnings = soldEvents.Sum(x => Maths.Discount(x.UnitsSold * x.UnitPrice, x.TotalDiscountPercent)),
                    AverageSaleDiscount = Maths.SafeAverage(soldEvents, x => x.TotalDiscountPercent),
                    SellingPrice = c.Prices.First().UnitValue,
                    ElementCount = c.Excerpts.Count,
                    MakingCosts = makingCosts
                };
            }).ToList();

            return HandlerResult<List<CompositeStatisticsDto>>.Ok(result);
        }
    }
}
