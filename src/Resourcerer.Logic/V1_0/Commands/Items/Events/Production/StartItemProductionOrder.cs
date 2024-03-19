using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos;
using Resourcerer.Logic.Exceptions;

using QU = Resourcerer.DataAccess.Utilities.Query.Instances;

namespace Resourcerer.Logic.V1_0.Commands.Items;

public static class StartItemProductionOrder
{
    public class Handler : IAppEventHandler<StartItemProductionOrderRequestDto, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(StartItemProductionOrderRequestDto request)
        {
            var order = await _dbContext.ItemProductionOrders
                .FirstOrDefaultAsync(x => x.Id == request.ProductionOrderId);

            if(order == null)
            {
                return HandlerResult<Unit>.NotFound($"Production order {request.ProductionOrderId} not found");
            }

            if (order.InstancesUsedIds.Length == 0)
            {
                var message = $"{nameof(order.InstancesUsedIds)} is empty for {nameof(ItemProductionOrder)} {order.Id}";
                throw new DataCorruptionException(message);
            }

            if (order.StartedEvent != null)
            {
                return HandlerResult<Unit>.Ok(Unit.New);
            }

            var instances = await _dbContext.Instances
                .Where(x => order.InstancesUsedIds.Contains(x.Id))
                .Select(QU.Expand(x => new Instance
                {
                    ReservedEventsJson = x.ReservedEventsJson
                }))
                .AsNoTracking()
                .ToArrayAsync();

            if (order.InstancesUsedIds.Length != instances.Length)
            {
                var message = $"Not all used instances found for consumption {nameof(ItemProductionOrder)} {order.Id}";
                throw new DataCorruptionException(message);
            }

            order.StartedEvent = JsonEntityBase.CreateEntity(() => new ItemProductionStartedEvent());

            foreach (var i in instances)
            {
                var reservationEvent = i.ReservedEvents
                    .First(x =>
                        x.ProductionOrderId == order.Id &&
                        x.CancelledEvent == null &&
                        x.UsedEvent == null);

                reservationEvent.UsedEvent = JsonEntityBase.CreateEntity(() => new InstanceReserveUsedEvent());

                _dbContext.Update(i);
            }

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }
    }

    public static ValidationResult Validate(StartItemProductionOrderRequestDto request) =>
        new Validator().Validate(request);

    public class Validator : AbstractValidator<StartItemProductionOrderRequestDto>
    {
        public Validator()
        {
            RuleFor(x => x.ProductionOrderId)
                .NotEmpty().WithMessage("Production order id cannot be empty");
        }
    }
}
