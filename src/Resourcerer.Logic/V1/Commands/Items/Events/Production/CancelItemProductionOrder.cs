using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;

namespace Resourcerer.Logic.V1.Commands.Items;
public static class CancelItemProductionOrder
{
    public class Handler : IAppEventHandler<V1CancelItemProductionOrderRequest, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1CancelItemProductionOrderRequest request)
        {
            var orderEvent = await _dbContext.ItemProductionOrders
                .FirstOrDefaultAsync(x => x.Id == request.ProductionOrderEventId);
            
            if(orderEvent == null)
            {
                return HandlerResult<Unit>.NotFound("Order event not found");
            }
            
            if(orderEvent.InstancesUsedIds.Length == 0)
            {
                var message = $"{nameof(orderEvent.InstancesUsedIds)} is empty for {nameof(ItemProductionOrder)} {orderEvent.Id}";
                throw new DataCorruptionException(message);
            }

            if (orderEvent.StartedEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Production already started");
            }

            var reservedEvents = await _dbContext.InstanceReservedEvents
                .Where(x => orderEvent.InstancesUsedIds.Contains(x.InstanceId))
                .ToArrayAsync();

            foreach(var ev in reservedEvents)
            {
                ev.CancelledEvent = AppDbJsonField.Create(() =>
                    new InstanceReserveCancelledEvent() { Reason = request.Reason });
            }

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        public static ValidationResult Validate(V1CancelItemProductionOrderRequest request) =>
            new Validator().Validate(request);

        private class Validator : AbstractValidator<V1CancelItemProductionOrderRequest>
        {
            public Validator()
            {
                RuleFor(x => x.ProductionOrderEventId)
                    .NotEmpty().WithMessage("Order event id cannot be empty");
            }
        }
    }
}
