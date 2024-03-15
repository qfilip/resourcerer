using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1_0.Functions;

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
                return HandlerResult<Unit>.Rejected("Item not found");
            }

            var productionOrder = new ItemProductionOrder
            {
                Id = Guid.NewGuid(),
                ItemId = item.Id,
                Quantity = request.Quantity
            };

            if (request.InstanceToUseIds.Length > 0)
            {
                var excerpts = await _dbContext.Excerpts
                .Where(x => x.CompositeId == request.ItemId)
                    .Include(x => x.Element)
                        .ThenInclude(x => x!.Instances)
                .AsNoTracking()
                .ToArrayAsync();

                //var instances = excerpts
                //    .SelectMany(x => x.Element!.Instances)
                //    .ToArray();

                var availableInstances = excerpts
                    .SelectMany(x => x.Element!.Instances, (e, x) =>
                    {
                        var available = Instances.GetAvailableUnitsInStock(x);
                        if (available < e.Quantity * request.Quantity)
                        {
                            return null;
                        }
                        else
                        {
                            return new
                            {
                                x.Id,
                                AvailableQuantity = Instances.GetAvailableUnitsInStock(x),
                                RequiredQuantity = e.Quantity * request.Quantity
                            };
                        }
                    })
                    .Where(x => x != null)
                    .ToList();

                // all instances exist
                var availableInstanceIds = availableInstances.Select(x => x!.Id).ToList();
                if(!request.InstanceToUseIds.All(id => availableInstanceIds.Contains(id)))
                {
                    return HandlerResult<Unit>.Rejected("Not all requested instances found");
                }

                var instancesToUpdate = await _dbContext.Instances
                    .Where(x => availableInstanceIds.Contains(x.Id))
                    .ToArrayAsync();

                foreach (var instance in instancesToUpdate)
                {
                    var requiredQuantity = availableInstances
                        .First(x => x!.Id == instance.Id)!
                        .RequiredQuantity;

                    var reservedEvent = JsonEntityBase.CreateEntity(() =>
                        new InstanceReservedEvent
                        {
                            ProductionOrderId = productionOrder.Id,
                            Quantity = requiredQuantity,
                            Reason = $"Production of item: {item.Id}-{item.Name}"
                        });
                    
                    instance.ReservedEvents.Add(reservedEvent);
                }
            }

            _dbContext.ItemProductionOrders.Add(productionOrder);
            
            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        public ValidationResult Validate(CreateItemProductionOrderRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}

