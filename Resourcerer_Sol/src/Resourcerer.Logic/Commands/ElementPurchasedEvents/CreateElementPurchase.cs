using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.ElementPurchasedEvents;

namespace Resourcerer.Logic.Commands.ElementPurchasedEvents;

public static class CreateElementPurchase
{
    public class Handler : IRequestHandler<CreateElementPurchaseDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;
        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(CreateElementPurchaseDto request)
        {
            var element = await _appDbContext.Elements
                .Include(x => x.UnitOfMeasure)
                .FirstOrDefaultAsync(x => x.Id == request.ElementId);

            if(element == null)
            {
                return HandlerResult<Unit>
                    .NotFound($"Element with id: {request.ElementId} not found");
            }

            var entity = new ElementPurchasedEvent
            {
                ElementId = element.Id,
                UnitOfMeasure = element.UnitOfMeasure,
                UnitPrice = request.UnitPrice,
                UnitsBought = request.UnitsBought,
                TotalDiscountPercent = request.TotalDiscountPercent
            };

            _appDbContext.ElementPurchasedEvents.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
