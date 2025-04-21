using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;

namespace Resourcerer.Logic.V1.Items.Events.Production;
public static class CreateCompositeItemProductionOrder
{
    public class Handler : IAppEventHandler<V1CreateCompositeItemProductionOrderCommand, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1CreateCompositeItemProductionOrderCommand request)
        {
            var item = _dbContext.Items
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Category!.CompanyId,
                    x.ExpirationTimeSeconds,
                    IsComposite = x.Recipes.Count > 0,
                })
                .FirstOrDefault(x =>
                    x.Id == request.ItemId &&
                    x.CompanyId == request.CompanyId);

            if (item == null)
                return HandlerResult<Unit>.NotFound("Item not found");

            if(!item.IsComposite)
                return HandlerResult<Unit>.Rejected("Item is not of composite type");

            var recipe = await _dbContext.Recipes
                .Where(x => x.CompositeItemId == item.Id)
                .OrderByDescending(x => x.Version)
                    .Include(x => x.RecipeExcerpts)
                        .ThenInclude(x => x.Element)
                            .ThenInclude(x => x!.Instances)
                                .ThenInclude(x => x.OrderedEvents)
                    .Include(x => x.RecipeExcerpts)
                        .ThenInclude(x => x.Element)
                            .ThenInclude(x => x!.Instances)
                                .ThenInclude(x => x.ReservedEvents)
                    .Include(x => x.RecipeExcerpts)
                        .ThenInclude(x => x.Element)
                            .ThenInclude(x => x!.Instances)
                                .ThenInclude(x => x.DiscardedEvents)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (recipe == null)
                throw new DataCorruptionException($"No recipes found for composite item {item.Id}");

            var excerpts = recipe.RecipeExcerpts;

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

            if (!specifiedInstancesExist)
            {
                return HandlerResult<Unit>.NotFound("Specified instances to use, not found");
            }

            var specifiedInstancesOwnedByCompany = allInstances
                .All(x => x.OwnerCompanyId == request.CompanyId);

            if (!specifiedInstancesOwnedByCompany)
            {
                return HandlerResult<Unit>.Rejected("Not all specified instances belong to the company");
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

            if (!correctlySpecifiedQuantities)
            {
                return HandlerResult<Unit>.Rejected("Specified quantities do not add up correctly");
            }

            var elementInstances = allInstances
                .ToLookup(x => x.ItemId);

            var hasResources = elementQuantityMap.All(qm =>
            {
                return elementInstances[qm.ElementId]
                    .Select(Functions.Instances.GetAvailableUnitsInStock)
                    .Sum() >= qm.Quantity * request.Quantity;
            });

            if (!hasResources)
            {
                return HandlerResult<Unit>.Rejected("Insufficient amount of resources available for item production");
            }

            var enoughSpecifiedInstancesInStock = request.InstancesToUse.All(x =>
            {
                var instance = allInstances.FirstOrDefault(i => i.Id == x.Key);
                if (instance == null)
                    return false;

                var availableQuantity = Functions.Instances.GetAvailableUnitsInStock(instance);
                return availableQuantity >= x.Value;
            });

            if (!enoughSpecifiedInstancesInStock)
            {
                return HandlerResult<Unit>.Rejected("Incorecctly specified instances");
            }

            var instanceToUpdateIds = request.InstancesToUse
                .Select(x => x.Key)
                .ToArray();

            var instancesToUpdate = await _dbContext.Instances
                .Where(x => instanceToUpdateIds.Contains(x.Id))
                .ToArrayAsync();

            var order = new ItemProductionOrder
            {
                Id = Guid.NewGuid(),
                ItemId = item.Id,
                CompanyId = request.CompanyId,
                Quantity = request.Quantity,
                Reason = request.Reason,
                ItemRecipeVersion = recipe.Version,
                InstancesUsedIds = instanceToUpdateIds
            };

            foreach (var instance in instancesToUpdate)
            {
                var reserveQuantity = request.InstancesToUse[instance.Id];

                var reservedEvent = new InstanceReservedEvent
                {
                    ItemProductionOrderId = order.Id,
                    InstanceId = instance.Id,
                    Quantity = reserveQuantity,
                    Reason = $"Production of item: {item.Id}-{item.Name}"
                };

                _dbContext.InstanceReservedEvents.Add(reservedEvent);
            }

            if(request.InstantProduction)
            {
                order.StartedEvent = AppDbJsonField.CreateKeyless(() => new ItemProductionStartedEvent());
                order.FinishedEvent = AppDbJsonField.CreateKeyless(() => new ItemProductionFinishedEvent());

                var expiration = item.ExpirationTimeSeconds;
                var newInstance = new Instance
                {
                    Quantity = order.Quantity,
                    ExpiryDate = Functions.Instances.GetExpirationDate(expiration, DateTime.UtcNow),

                    ItemId = item.Id,
                    OwnerCompanyId = order.CompanyId
                };

                _dbContext.Instances.Add(newInstance);
            }

            _dbContext.ItemProductionOrders.Add(order);

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }
    }
    public class Validator : AbstractValidator<V1CreateCompositeItemProductionOrderCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage("Item id cannot be empty");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Company id cannot be empty");

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

