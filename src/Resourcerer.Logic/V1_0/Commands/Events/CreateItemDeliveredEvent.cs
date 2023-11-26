using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.V1_0;

public static class CreateItemDeliveredEvent
{
    public class Handler : IAppHandler<ItemDeliveredEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(ItemDeliveredEventDto request)
        {
            var orderEvent = await _appDbContext.ItemOrderedEvents
                .Include(x => x.ItemOrderCancelledEvent)
                .Include(x => x.ItemDeliveredEvent)
                .FirstOrDefaultAsync(x => x.Id == request.InstanceOrderedEventId);

            if (orderEvent == null)
            {
                var message = $"Order event with id {request.InstanceOrderedEventId} not found";
                return HandlerResult<Unit>.Rejected(message);
            }

            if (orderEvent.ItemOrderCancelledEvent != null)
            {
                var error = "Order was cancelled, and cannot be delivered";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.ItemDeliveredEvent != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var deliveredEvent = new ItemDeliveredEvent
            {
                ItemOrderedEventId = orderEvent.Id
            };

            _appDbContext.ItemDeliveredEvents.Add(deliveredEvent);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(ItemDeliveredEventDto request)
        {
            throw new NotImplementedException();
        }
    }
}
