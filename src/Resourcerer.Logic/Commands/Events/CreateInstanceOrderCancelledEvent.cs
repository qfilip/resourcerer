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
            var orderedEvent = await _appDbContext.InstanceBoughtEvents
                .Include(x => x.InstanceCancelledEvent)
                .Include(x => x.InstanceDeliveredEvent)
                .FirstOrDefaultAsync(x => x.Id == request.InstanceOrderedEventId);

            if (orderedEvent == null)
            {
                var error = $"Order event with id {request.InstanceOrderedEventId} not found";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderedEvent.InstanceDeliveredEvent != null)
            {
                var error = "Order was delivered, and cannot be cancelled";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderedEvent.InstanceCancelledEventId != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var entity = new InstanceCancelledEvent
            {
                InstanceBoughtEventId = orderedEvent.Id
            };

            _appDbContext.InstanceCancelledEvents.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
