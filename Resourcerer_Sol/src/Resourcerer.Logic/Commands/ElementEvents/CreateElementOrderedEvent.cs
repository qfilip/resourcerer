using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.ElementEvents;

public static class CreateElementOrderedEvent
{
    public class Handler : IAppHandler<InstanceOrderedEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(InstanceOrderedEventDto request)
        {
            if(request.ElementId == null)
            {
                return HandlerResult<Unit>
                    .ValidationError($"ElementId cannot be null");
            }

            var element = await _appDbContext.Items
                .Include(x => x.UnitOfMeasure)
                .Include(x => x.Behavior)
                .FirstOrDefaultAsync(x => x.Id == request.ElementId);

            if (element == null)
            {
                return HandlerResult<Unit>
                    .ValidationError($"Element with id {request.ElementId} not found");
            }

            if(element.Behavior != null && element.Behavior.ExpirationTime != null)
            {
                if(request.ExpectedDeliveryDate == null)
                {
                    return HandlerResult<Unit>
                        .ValidationError($"Expected delivery time must be set for elements that can expire");
                }

                if (request.ExpiryDate == null)
                {
                    return HandlerResult<Unit>
                        .ValidationError($"Expiry date must be set for elements that can expire");
                }

                if(request.ExpectedDeliveryDate <= request.ExpiryDate)
                {
                    return HandlerResult<Unit>
                        .ValidationError($"Ordered items will expire before they are delivered");
                }
            }

            var instance = new Instance
            {
                ElementId = element.Id,
                ExpiryDate = request.ExpiryDate
            };

            var entity = new InstanceOrderedEvent
            {
                Instance = instance,
                UnitPrice = request.UnitPrice,
                UnitsOrdered = request.UnitsOrdered,
                TotalDiscountPercent = request.TotalDiscountPercent,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate
            };

            _appDbContext.InstanceOrderedEvents.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
