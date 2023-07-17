using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.Events;

public static class CreateInstanceOrderCancelledEvent
{
    public class Handler : IAppHandler<InstanceOrderCancelledEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(InstanceOrderCancelledEventDto request)
        {
            var orderedEvent = await _appDbContext.InstanceOrderedEvents
                .Include(x => x.InstanceRequestCancelledEvent)
                .Include(x => x.InstanceRequestDeliveredEvent)
                .FirstOrDefaultAsync(x => x.Id == request.InstanceOrderedEventId);

            if (orderedEvent == null)
            {
                var error = $"Order event with id {request.InstanceOrderedEventId} not found";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderedEvent.InstanceRequestDeliveredEvent != null)
            {
                var error = "Order was delivered, and cannot be cancelled";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderedEvent.InstanceRequestCancelledEventId != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var entity = new InstanceRequestCancelledEvent
            {
                InstanceBuyRequestedEventId = orderedEvent.Id
            };

            _appDbContext.InstanceOrderCancelledEvents.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
