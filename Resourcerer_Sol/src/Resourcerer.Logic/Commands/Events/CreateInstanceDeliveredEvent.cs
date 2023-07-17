using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.Events;

public static class CreateInstanceDeliveredEvent
{
    public class Handler : IAppHandler<InstanceDeliveredEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(InstanceDeliveredEventDto request)
        {
            var orderEvent = await _appDbContext.InstanceOrderedEvents
                .Include(x => x.InstanceRequestCancelledEvent)
                .Include(x => x.InstanceRequestDeliveredEvent)
                .FirstOrDefaultAsync(x => x.Id == request.InstanceOrderedEventId);

            if (orderEvent == null)
            {
                var message = $"Order event with id {request.InstanceOrderedEventId} not found";
                return HandlerResult<Unit>.Rejected(message);
            }

            if (orderEvent.InstanceRequestCancelledEvent != null)
            {
                var error = "Order was cancelled, and cannot be delivered";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.InstanceRequestDeliveredEvent != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var deliveredEvent = new InstanceRequestDeliveredEvent
            {
                InstanceBuyRequestedEventId = orderEvent.Id
            };

            _appDbContext.InstanceOrderDeliveredEvents.Add(deliveredEvent);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
