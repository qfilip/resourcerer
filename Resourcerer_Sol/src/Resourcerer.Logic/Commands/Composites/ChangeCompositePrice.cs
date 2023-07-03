using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Prices;

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

            var price = new Price
            {
                CompositeId = composite.Id,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Prices.Add(price);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
