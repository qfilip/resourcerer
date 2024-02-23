using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1_0.Functions;

namespace Resourcerer.Logic.V1_0.Commands.Items;
public static class CreateItemProductionOrderedEvent
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

            var productionOrder = JsonEntityBase.CreateEntity(() =>
            {
                return new ItemProductionOrderedEvent
                {
                    Quantity = request.Quantity
                };
            });

            item.ProductionOrderedEvents.Add(productionOrder);
            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        private static async Task<bool> ValidateRequestedInstancesAsync(Guid itemId, Guid[] requestedInstanceIds, AppDbContext appDbContext)
        {
            var excerpts = await appDbContext.Excerpts
                .Where(x => x.CompositeId == itemId)
                    .Include(x => x.Element)
                        .ThenInclude(x => x!.Instances)
                .ToArrayAsync();

            var instances = excerpts
                .SelectMany(x => x.Element!.Instances)
                .ToArray();

            var usableInstances = instances
                .Select(x => new
                {
                    Id = x.Id,
                    Quantity = Instances.GetAvailableUnitsInStock(x)
                })
                .ToArray();

            throw new NotImplementedException();
        }

        public ValidationResult Validate(CreateItemProductionOrderRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
