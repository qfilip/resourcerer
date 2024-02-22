using FluentValidation.Results;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.Logic.V1_0.Commands.Items;
public static class CreateItemProductionOrderedEvent
{
    public class Handler : IAppHandler<Unit, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(Unit request)
        {
            var productionOrder = JsonEntityBase.CreateEntity(() =>
            {
                return new ItemProductionOrderedEvent
                {
                    Quantity = 0
                };
            });
        }

        public ValidationResult Validate(Unit request)
        {
            throw new NotImplementedException();
        }
    }
}
