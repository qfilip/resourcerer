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
                .Include(x => x.InstanceOrderCancelledEvent)
                .Include(x => x.InstanceOrderDeliveredEvent)
                .FirstOrDefaultAsync(x => x.Id == request.InstanceOrderedEventId);

            if (orderEvent == null)
            {
                var message = $"Order event with id {request.InstanceOrderedEventId} not found";
                return HandlerResult<Unit>.ValidationError(message);
            }

            if (orderEvent.InstanceOrderCancelledEvent != null)
            {
                var error = "Order was cancelled, and cannot be delivered";
                return HandlerResult<Unit>.ValidationError(error);
            }

            if (orderEvent.InstanceOrderDeliveredEvent != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var deliveredEvent = new InstanceOrderDeliveredEvent
            {
                InstanceOrderedEventId = orderEvent.Id
            };

            _appDbContext.InstanceOrderDeliveredEvents.Add(deliveredEvent);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
