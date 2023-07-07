using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.ElementEvents;

public static class CreateElementPurchaseCancelledEvent
{
    public class Handler : IAppHandler<ElementPurchaseCancelledEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(ElementPurchaseCancelledEventDto request)
        {
            var purchaseEvent = await _appDbContext.ElementPurchasedEvents
                .Include(x => x.ElementDeliveredEvent)
                .Include(x => x.ElementPurchaseCancelledEvent)
                .FirstOrDefaultAsync(x => x.Id == request.ElementPurchasedEventId);

            if(purchaseEvent == null)
            {
                var error = $"Purchase event with id {request.ElementPurchasedEventId} not found";
                return HandlerResult<Unit>.ValidationError(error);
            }

            if (purchaseEvent.ElementDeliveredEvent != null)
            {
                var error = "Purchase was delivered, and cannot be cancelled";
                return HandlerResult<Unit>.ValidationError(error);
            }

            if (purchaseEvent.ElementPurchaseCancelledEvent != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var entity = new ElementPurchaseCancelledEvent
            {
                ElementPurchasedEventId = request.ElementPurchasedEventId
            };

            _appDbContext.ElementPurchaseCancelledEvents.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
