using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.V1_0;

public static class CreateItemOrderedEvent
{
    public class Handler : IAppHandler<ItemOrderedEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(ItemOrderedEventDto request)
        {
            var item = await _appDbContext.Items
                .Include(x => x.UnitOfMeasure)
                .FirstOrDefaultAsync(x => x.Id == request.ItemId);

            if (item == null)
            {
                return HandlerResult<Unit>
                    .Rejected($"Item with id {request.ItemId} not found");
            }

            if(item.ExpirationTimeSeconds != null)
            {
                if(request.ExpectedDeliveryDate == null)
                {
                    return HandlerResult<Unit>
                        .Rejected($"Expected delivery time must be set for items that can expire");
                }

                if (request.ExpiryDate == null)
                {
                    return HandlerResult<Unit>
                        .Rejected($"Expiry date must be set for items that can expire");
                }

                if(request.ExpectedDeliveryDate >= request.ExpiryDate)
                {
                    return HandlerResult<Unit>
                        .Rejected($"Ordered items will expire before they are delivered");
                }
            }

            var instance = new Instance
            {
                Id = Guid.NewGuid(),
                ExpiryDate = request.ExpiryDate,
                
                ItemId = item.Id
            };

            var orderedEvent = new ItemOrderedEvent
            {
                InstanceId = instance.Id,
                UnitPrice = request.UnitPrice,
                Quantity = request.UnitsOrdered,
                TotalDiscountPercent = request.TotalDiscountPercent,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate,
            };

            _appDbContext.InstanceBoughtEvents.Add(orderedEvent);
            _appDbContext.Instances.Add(instance);
            
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
