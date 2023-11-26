using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic.Exceptions;

namespace Resourcerer.Logic.V1_0.Commands;

public static class CreateItemDiscardedEvent
{
    public class Handler : IAppHandler<ItemDiscardedEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(ItemDiscardedEventDto request)
        {
            var instance = await _appDbContext.Instances
                    .Include(x => x.ItemOrderedEvent)
                    .Include(x => x.ItemSoldEvents)
                    .Include(x => x.ItemDiscardedEvents)
                .FirstOrDefaultAsync(x => x.Id == request.InstanceId);

            var noInstance =
                instance == null ||
                instance.ItemOrderedEvent!.ItemOrderCancelledEventId != null;

            if (noInstance)
            {
                return HandlerResult<Unit>.NotFound($"Instance with id {request.InstanceId} not found");
            }

            var quantity = instance!.ItemOrderedEvent!.Quantity;
            
            var quantitySpent =
                instance!.ItemSoldEvents.Sum(x => x.Quantity) +
                instance!.ItemDiscardedEvents.Sum(x => x.Quantity);

            var quantityLeft = quantity - quantitySpent;
            if (quantityLeft < 0)
            {
                var error = $"More instances spent than ordered for instance id: {instance!.Id}";
                throw new DataCorruptionException(error);
            }
            else if (quantityLeft == 0)
            {
                return HandlerResult<Unit>.Rejected("Remaining quantity is 0. Nothing to discard.");
            }
            else if (quantityLeft - request.Quantity < 0)
            {
                return HandlerResult<Unit>.Rejected($"Remaining quantity is 0. Cannot discard {request.Quantity} number of items");
            }
            else
            {
                var entity = new ItemDiscardedEvent
                {
                    InstanceId = instance!.Id,
                    Quantity = request.Quantity,
                    Reason = request.Reason
                };

                _appDbContext.ItemDiscardedEvents.Add(entity);
                await _appDbContext.SaveChangesAsync();

                return HandlerResult<Unit>.Ok(Unit.New);
            }
        }
    }
}
