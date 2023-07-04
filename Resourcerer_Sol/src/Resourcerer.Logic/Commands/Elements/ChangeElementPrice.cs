using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.Elements;

public class ChangeElementPrice
{
    public class Handler : IRequestHandler<ChangePriceDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;
        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(ChangePriceDto request)
        {   
            var prices = await _appDbContext.Prices
                .Where(x => x.ElementId == request.EntityId)
                .ToListAsync();

            if(!prices.Any())
            {
                return HandlerResult<Unit>.NotFound($"Price for entity with id {request.EntityId} not found");
            }

            if(prices.Count > 1)
            {
                // report error
            }

            prices.ForEach(x => x.EntityStatus = eEntityStatus.Deleted);

            var price = new Price
            {
                ElementId = request.EntityId,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Prices.Add(price);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
