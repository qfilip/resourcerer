using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.Elements.Events;

public static class CreateElementPurchaseCancelledEvent
{
    public class Handler : IAppHandler<ElementPurchaseCancelledEventDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;
        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(ElementPurchaseCancelledEventDto request)
        {
            var deliveryEvent = await _appDbContext.ElementDeliveredEvents
                .FirstOrDefaultAsync(x => x.ElementPurchasedEventId == request.ElementPurchasedEventId);

            if (deliveryEvent != null)
            {
                var error = "Purchase was delivered, and cannot be cancelled";
                return HandlerResult<Unit>.ValidationError(error);
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
