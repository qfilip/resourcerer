using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.V1_0;

public static class CreateItemOrderCancelledEvent
{
    public class Handler : IAppHandler<ItemCancelledEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(ItemCancelledEventDto request)
        {
            var orderedEvent = await _appDbContext.ItemOrderedEvents
                .Include(x => x.ItemOrderCancelledEvent)
                .Include(x => x.ItemDeliveredEvent)
                .FirstOrDefaultAsync(x => x.Id == request.TargetEventId);

            if (orderedEvent == null)
            {
                var error = $"Order event with id {request.TargetEventId} not found";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderedEvent.ItemDeliveredEvent != null)
            {
                var error = "Order was delivered, and cannot be cancelled";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderedEvent.ItemOrderCancelledEventId != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var entity = new ItemCancelledEvent
            {
                InstanceBoughtEventId = orderedEvent.Id
            };

            _appDbContext.ItemCancelledEvents.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(ItemCancelledEventDto request)
        {
            throw new NotImplementedException();
        }
    }
}
