using FluentValidation.Results;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos;

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

        public ValidationResult Validate(CreateItemProductionOrderRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
