using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.V1_0.Commands.Items;
public static class FinishItemProductionOrder
{
    public class Handler : IAppEventHandler<FinishItemProductionOrderRequest, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(FinishItemProductionOrderRequest request)
        {
            var order = await _dbContext.ItemProductionOrders
                .FirstOrDefaultAsync(x => x.Id == request.ProductionOrderId);

            if (order == null)
            {
                return HandlerResult<Unit>.NotFound($"Production order {request.ProductionOrderId} not found");
            }

            if(order.CanceledEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Production order was cancelled");
            }

            if (order.StartedEvent == null)
            {
                return HandlerResult<Unit>.Rejected("Production order has not started yet");
            }

            order.FinishedEvent = JsonEntityBase.CreateEntity(() => new ItemProductionFinishedEvent());

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }
    }

    public static ValidationResult Validate(FinishItemProductionOrderRequest request) =>
        new Validator().Validate(request);

    public class Validator : AbstractValidator<FinishItemProductionOrderRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ProductionOrderId)
                .NotEmpty().WithMessage("Production order id cannot be empty");
        }
    }
}
