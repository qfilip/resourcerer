using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Prices;

namespace Resourcerer.Logic.Commands.Prices;

public static class ChangePrice
{
    public class Handler : IRequestHandler<PriceDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;
        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(PriceDto request)
        {
            var composite = await _appDbContext.Composites
                .FirstOrDefaultAsync(x => x.Id == request.CompositeId);

            if (composite == null)
            {
                HandlerResult<Unit>.ValidationError("Cannot find supplied composite");
            }

            var entity = new Price
            {
                CompositeId = request.CompositeId,
                UnitValue = request.Value
            };

            _appDbContext.Prices.Add(entity);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
