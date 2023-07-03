using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Prices;

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
            var element = await _appDbContext.Elements
                .Include(x => x.Prices)
                .FirstOrDefaultAsync(x => x.Id == request.EntityId);

            if (element == null)
            {
                return HandlerResult<Unit>.NotFound($"Entity with id {request.EntityId} not found");
            }

            var price = new Price
            {
                ElementId = element.Id,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Prices.Add(price);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
