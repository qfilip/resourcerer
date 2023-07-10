using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.ElementEvents;

public static class CreateElementDeliveredEvent
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
                .Include(x => x.InstanceDeliveredEvent)
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

            if (orderEvent.InstanceDeliveredEvent != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var deliveredEvent = new InstanceDeliveredEvent
            {
                InstanceOrderedEventId = orderEvent.Id
            };

            _appDbContext.InstanceDeliveredEvents.Add(deliveredEvent);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
