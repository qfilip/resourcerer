using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos;
using Resourcerer.Logic.Exceptions;

namespace Resourcerer.Logic.V1_0.Commands.Items;
public static class CancelItemProductionOrder
{
    public class Handler : IAppEventHandler<CancelItemProductionOrderRequestDto, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CancelItemProductionOrderRequestDto request)
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

            var instances = await _dbContext.Instances
                .Where(x => orderEvent.InstancesUsedIds.Contains(x.Id))
                .ToArrayAsync();

            if (orderEvent.InstancesUsedIds.Length != instances.Length)
            {
                var message = $"Not all used instances found for cancelling {nameof(ItemProductionOrder)} {orderEvent.Id}";
                throw new DataCorruptionException(message);
            }

            orderEvent.CanceledEvent = JsonEntityBase.CreateEntity(() =>
                new ItemProductionOrderCancelledEvent
                {
                    Reason = request.Reason
                });

            foreach(var i in instances)
            {
                var reservationEvent = i.ReservedEvents
                    .First(x =>
                        x.ProductionOrderId == orderEvent.Id &&
                        x.CancelledEvent == null &&
                        x.UsedEvent == null);

                reservationEvent.CancelledEvent = JsonEntityBase.CreateEntity(() =>
                    new InstanceReserveCancelledEvent
                    {
                        Reason = request.Reason
                    });
            }

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        public static ValidationResult Validate(CancelItemProductionOrderRequestDto request) =>
            new Validator().Validate(request);

        private class Validator : AbstractValidator<CancelItemProductionOrderRequestDto>
        {
            public Validator()
            {
                RuleFor(x => x.ProductionOrderEventId)
                    .NotEmpty().WithMessage("Order event id cannot be empty");
            }
        }
    }
}
