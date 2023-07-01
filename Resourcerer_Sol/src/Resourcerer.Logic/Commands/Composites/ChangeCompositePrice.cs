using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.OldPrices;

namespace Resourcerer.Logic.Commands.Composites;

public static class ChangeCompositePrice
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
            var composite = await _appDbContext.Composites
                .FirstOrDefaultAsync(x => x.Id == request.EntityId);

            if(composite == null)
            {
                return HandlerResult<Unit>.NotFound($"Entity with id {request.EntityId} not found");
            }

            var oldPrice = new OldPrice
            {
                CompositeId = composite.Id,
                UnitValue = composite.CurrentSellPrice
            };

            composite.CurrentSellPrice = request.NewPrice;

            _appDbContext.OldPrices.Add(oldPrice);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
