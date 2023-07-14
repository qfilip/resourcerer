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
            var item = await _appDbContext.Items
                .Include(x => x.UnitOfMeasure)
                .FirstOrDefaultAsync(x => x.Id == request.ItemId);

            if (item == null)
            {
                return HandlerResult<Unit>
                    .ValidationError($"Item with id {request.ItemId} not found");
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

                if(request.ExpectedDeliveryDate >= request.ExpiryDate)
                {
                    return HandlerResult<Unit>
                        .ValidationError($"Ordered items will expire before they are delivered");
                }
            }

            var instance = new Instance
            {
                Id = Guid.NewGuid(),

                UnitPrice = request.UnitPrice,
                UnitsOrdered = request.UnitsOrdered,
                TotalDiscountPercent = request.TotalDiscountPercent,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate,
                ExpiryDate = request.ExpiryDate,
                
                ItemId = item.Id
            };

            var orderedEvent = new InstanceBuyRequestedEvent
            {
                InstanceId = instance.Id,
                OrderType = request.OrderType
            };

            _appDbContext.InstanceOrderedEvents.Add(orderedEvent);
            _appDbContext.Instances.Add(instance);
            
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
