using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;
using System.Diagnostics;

namespace Resourcerer.Logic.Commands.Composites;

public static class ChangeCompositePrice
{
    public class Handler : IAppHandler<ChangePriceDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(ChangePriceDto request)
        {
            var composite = await _appDbContext.Composites
                .Include(x => x.Prices)
                .FirstOrDefaultAsync(x => x.Id == request.EntityId);

            if(composite == null)
            {
                var error = $"Composite with id {request.EntityId} doesn't exist";
                return HandlerResult<Unit>.ValidationError(error);
            }

            foreach (var p in composite.Prices)
                p.EntityStatus = eEntityStatus.Deleted;

            var price = new Price
            {
                CompositeId = request.EntityId,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Prices.Add(price);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
