using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using System.Diagnostics;

namespace Resourcerer.Logic.Commands.ElementEvents;

public static class CreateElementDeliveredEvent
{
    public class Handler : IAppHandler<CreateElementDeliveredEventDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;
        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CreateElementDeliveredEventDto request)
        {
            var purchaseEvent = await _appDbContext.ElementPurchasedEvents
                .Include(x => x.ElementPurchaseCancelledEvent)
                .Include(x => x.ElementPurchaseCancelledEvent)
                .FirstOrDefaultAsync(x => x.Id == request.ElementPurchasedEventId);
            
            if (purchaseEvent == null)
            {
                var message = $"Purchase event with id {request.ElementPurchasedEventId} not found";
                return HandlerResult<Unit>.NotFound(message);
            }

            if (purchaseEvent.ElementPurchaseCancelledEvent != null)
            {
                var error = "Purchase was cancelled, and cannot be delivered";
                return HandlerResult<Unit>.ValidationError(error);
            }

            if(purchaseEvent.ElementDeliveredEvent != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var deliveredEvent = new ElementDeliveredEvent
            {
                ElementPurchasedEventId = request.ElementPurchasedEventId
            };

            var elementInstance = new Instance
            {
                ElementId = purchaseEvent.ElementId,
                Quantity = purchaseEvent.UnitsBought,
                ExpiryDate = request.InstanceExpiryDate
            };

            _appDbContext.ElementDeliveredEvents.Add(deliveredEvent);
            _appDbContext.Instances.Add(elementInstance);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
