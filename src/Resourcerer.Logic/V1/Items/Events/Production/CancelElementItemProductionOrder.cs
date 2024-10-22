using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1.Items.Events.Production;

public static class CancelElementItemProductionOrder
{
    public class Handler : IAppEventHandler<V1CancelElementItemProductionOrderCommand, Unit>
    {
        private readonly AppDbContext _dbContext;
        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1CancelElementItemProductionOrderCommand request)
        {
            var order = await _dbContext.ItemProductionOrders
                .FirstOrDefaultAsync(x => x.Id == request.ProductionOrderId);

            if (order == null)
            {
                return HandlerResult<Unit>.NotFound("Order event not found");
            }

            if (order.StartedEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Production already started");
            }

            if (order.FinishedEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Production already finished");
            }

            if(order.CancelledEvent != null)
            {
                return HandlerResult<Unit>.Ok(Unit.New);
            }

            order.CancelledEvent = AppDbJsonField.CreateKeyless(() =>
                new ItemProductionOrderCancelledEvent() { Reason = request.Reason });

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }
    }

    public class Validator : AbstractValidator<V1CancelElementItemProductionOrderCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ProductionOrderId)
                .NotEmpty().WithMessage("Order event id cannot be empty");
        }
    }
}
