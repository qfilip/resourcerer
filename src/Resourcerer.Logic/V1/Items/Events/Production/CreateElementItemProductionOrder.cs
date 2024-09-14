using FluentValidation;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1.Items.Events.Production;

public static class CreateElementItemProductionOrder
{
    public class Handler : IAppEventHandler<V1CreateElementItemProductionOrderCommand, Unit>
    {
        private readonly AppDbContext _dbContext;
        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1CreateElementItemProductionOrderCommand request)
        {
            var item = _dbContext.Items
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Category!.CompanyId,
                    x.ExpirationTimeSeconds
                })
                .FirstOrDefault(x =>
                    x.Id == request.ItemId &&
                    x.CompanyId == request.CompanyId);

            if (item == null)
            {
                return HandlerResult<Unit>.NotFound("Item not found");
            }

            var order = new ItemProductionOrder
            {
                Id = Guid.NewGuid(),
                ItemId = item.Id,
                CompanyId = request.CompanyId,
                Quantity = request.Quantity,
                Reason = request.Reason
            };

            if(request.InstantProduction)
            {
                order.StartedEvent = AppDbJsonField.Create(() => new ItemProductionStartedEvent());
                order.FinishedEvent = AppDbJsonField.Create(() => new ItemProductionFinishedEvent());

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

    public class Validator : AbstractValidator<V1CreateElementItemProductionOrderCommand>
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
        }
    }
}
