using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1.Items.Events.Production;
public static class FinishItemProductionOrder
{
    public class Handler : IAppEventHandler<V1FinishItemProductionOrderCommand, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1FinishItemProductionOrderCommand request)
        {
            var order = await _dbContext.ItemProductionOrders
                .Select(x => new ItemProductionOrder
                {
                    Id = x.Id,
                    Quantity = x.Quantity,
                    Item = new Item
                    {
                        Id = x.ItemId,
                        ExpirationTimeSeconds = x.Item!.ExpirationTimeSeconds
                    },
                    CompanyId = x.CompanyId,
                    CancelledEvent = x.CancelledEvent,
                    StartedEvent = x.StartedEvent,
                    FinishedEvent = x.FinishedEvent
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ProductionOrderId);

            if (order == null)
            {
                return HandlerResult<Unit>.NotFound($"Production order {request.ProductionOrderId} not found");
            }

            if (order.CancelledEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Production order was cancelled");
            }

            if (order.StartedEvent == null)
            {
                return HandlerResult<Unit>.Rejected("Production order has not started yet");
            }

            if (order.FinishedEvent != null)
            {
                return HandlerResult<Unit>.Ok(Unit.New);
            }

            _dbContext.ItemProductionOrders.Attach(order);

            order.FinishedEvent = AppDbJsonField.Create(() => new ItemProductionFinishedEvent());

            var expiration = order.Item!.ExpirationTimeSeconds;
            var newInstance = new Instance
            {
                Quantity = order.Quantity,
                ExpiryDate = Functions.Instances.GetExpirationDate(expiration, DateTime.UtcNow),

                ItemId = order.Item.Id,
                OwnerCompanyId = order.CompanyId
            };

            var itemProductionOrderUpdates = new ItemProductionOrder
            {
                Id = order.Id,
                FinishedEvent = AppDbJsonField.Create(() => new ItemProductionFinishedEvent())
            };

            _dbContext.Instances.Add(newInstance);

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }
    }
    public class Validator : AbstractValidator<V1FinishItemProductionOrderCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ProductionOrderId)
                .NotEmpty().WithMessage("Production order id cannot be empty");
        }
    }
}
