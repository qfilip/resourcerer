using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.Events;

public static class CreateInstanceOrderedEvent
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

            var item = await _appDbContext.Items
                .Include(x => x.UnitOfMeasure)
                .FirstOrDefaultAsync(x => x.Id == request.ElementId);

            if (item == null)
            {
                return HandlerResult<Unit>
                    .ValidationError($"Item with id {request.ElementId} not found");
            }

            if(item.ExpirationTimeSeconds != null)
            {
                if(request.ExpectedDeliveryDate == null)
                {
                    return HandlerResult<Unit>
                        .ValidationError($"Expected delivery time must be set for items that can expire");
                }

                if (request.ExpiryDate == null)
                {
                    return HandlerResult<Unit>
                        .ValidationError($"Expiry date must be set for items that can expire");
                }

                if(request.ExpectedDeliveryDate <= request.ExpiryDate)
                {
                    return HandlerResult<Unit>
                        .ValidationError($"Ordered items will expire before they are delivered");
                }
            }

            var instance = new Instance
            {
                ItemId = item.Id,
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
