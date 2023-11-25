using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.V1_0;

public static class CreateItemOrderCancelledEvent
{
    public class Handler : IAppHandler<ItemOrderCancelledEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(ItemOrderCancelledEventDto request)
        {
            var orderedEvent = await _appDbContext.InstanceBoughtEvents
                .Include(x => x.ItemSellCancelledEvent)
                .Include(x => x.ItemDeliveredEvent)
                .FirstOrDefaultAsync(x => x.Id == request.InstanceOrderedEventId);

            if (orderedEvent == null)
            {
                var error = $"Order event with id {request.InstanceOrderedEventId} not found";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderedEvent.ItemDeliveredEvent != null)
            {
                var error = "Order was delivered, and cannot be cancelled";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderedEvent.ItemSellCancelledEventId != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var entity = new ItemSellCancelledEvent
            {
                InstanceBoughtEventId = orderedEvent.Id
            };

            _appDbContext.InstanceCancelledEvents.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
