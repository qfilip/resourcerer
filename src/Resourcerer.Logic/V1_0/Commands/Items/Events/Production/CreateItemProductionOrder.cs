using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1_0.Functions;
using System.ComponentModel.Design;

namespace Resourcerer.Logic.V1_0.Commands.Items;
public static class CreateItemProductionOrder
{
    public class Handler : IAppHandler<CreateItemProductionOrderRequestDto, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CreateItemProductionOrderRequestDto request)
        {
            var item = _dbContext.Items
                .FirstOrDefault(x => x.Id == request.ItemId);

            if (item == null)
            {
                return HandlerResult<Unit>.NotFound("Item not found");
            }

            var excerpts = await _dbContext.Excerpts
                .Where(x => x.CompositeId == request.ItemId)
                    .Include(x => x.Element)
                        .ThenInclude(x => x!.Instances)
                .AsNoTracking()
                .ToArrayAsync();

            var elementQuantityMap = excerpts
                .Select(x => new
                {
                    x.ElementId,
                    x.Quantity
                });

            var allInstances = excerpts
                .SelectMany(x => x.Element!.Instances)
                .ToArray();

            var specifiedInstancesExist = allInstances
                .All(x => request.InstancesToUse.Keys.Contains(x.Id));

            if(!specifiedInstancesExist)
            {
                return HandlerResult<Unit>.NotFound("Specified instances to use, not found");
            }

            var correctlySpecifiedQuantities = elementQuantityMap.All(x =>
            {
                var ids = allInstances
                    .Where(i => i.ItemId == x.ElementId)
                    .Select(i => i.Id)
                    .ToArray();

                var kvs = request.InstancesToUse
                    .Where(kv => ids.Contains(kv.Key))
                    .ToArray();

                return kvs.Sum(kv => kv.Value) == x.Quantity * request.Quantity;
            });

            if(!correctlySpecifiedQuantities)
            {
                return HandlerResult<Unit>.Rejected("Specified quantities do not add up correctly");
            }

            var elementInstances = allInstances
                .ToLookup(x => x.ItemId);

            var hasResources = elementQuantityMap.All(qm =>
            {
                return elementInstances[qm.ElementId]
                    .Select(Instances.GetAvailableUnitsInStock)
                    .Sum() >= qm.Quantity * request.Quantity;
            });

            if(!hasResources)
            {
                return HandlerResult<Unit>.Rejected("Insufficient amount of resources available for item production");
            }

            var enoughSpecifiedInstancesInStock = request.InstancesToUse.All(x =>
            {
                var instance = allInstances.FirstOrDefault(i => i.Id == x.Key);
                if (instance == null)
                    return false;

                var availableQuantity = Instances.GetAvailableUnitsInStock(instance);
                return availableQuantity >= x.Value;
            });

            if(!enoughSpecifiedInstancesInStock)
            {
                return HandlerResult<Unit>.Rejected("Incorecctly specified instances");
            }

            var instanceToUpdateIds = request.InstancesToUse
                .Select(x => x.Key)
                .ToArray();

            var instancesToUpdate = await _dbContext.Instances
                .Where(x => instanceToUpdateIds.Contains(x.Id))
                .ToArrayAsync();

            var productionOrder = new ItemProductionOrder
            {
                Id = Guid.NewGuid(),
                ItemId = item.Id,
                Quantity = request.Quantity
            };

            foreach (var instance in instancesToUpdate)
            {
                var reserveQuantity = request.InstancesToUse[instance.Id];

                var reservedEvent = JsonEntityBase.CreateEntity(() =>
                    new InstanceReservedEvent
                    {
                        ProductionOrderId = productionOrder.Id,
                        Quantity = reserveQuantity,
                        Reason = $"Production of item: {item.Id}-{item.Name}"
                    });

                instance.ReservedEvents.Add(reservedEvent);
            }

            _dbContext.ItemProductionOrders.Add(productionOrder);

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        public ValidationResult Validate(CreateItemProductionOrderRequestDto request) =>
            new Validator().Validate(request);

        private class Validator : AbstractValidator<CreateItemProductionOrderRequestDto>
        {
            public Validator()
            {
                RuleFor(x => x.ItemId)
                    .NotEmpty().WithMessage("Item id cannot be empty");

                RuleFor(x => x.Quantity)
                    .Must(x => x > 0).WithMessage("Item production quantity must be greater than 0");

                RuleFor(x => x.DesiredProductionStartTime)
                    .Must(x => x >= DateTime.UtcNow).WithMessage("Desired production start time cannot be in the past");

                RuleFor(x => x.InstancesToUse)
                    .NotEmpty().WithMessage("Instances to use in production, not specified");

                RuleFor(x => x.InstancesToUse)
                    .Must(x => x.Values.All(v => v > 0))
                    .WithMessage("Quantities of all instances to use in production, must be above 0");

            }
        }
    }
}

