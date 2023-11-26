using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.V1_0.Commands;

public static class CreateItemSellCancelledEvent
{
    public class Handler : IAppHandler<ItemCancelledEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(ItemCancelledEventDto request)
        {
            var soldEvent = await _appDbContext.ItemSoldEvents
                .Include(x => x.ItemSellCancelledEvent)
                .Include(x => x.ItemDeliveredEvent)
                .FirstOrDefaultAsync(x => x.Id == request.TargetEventId);

            if (soldEvent == null)
            {
                return HandlerResult<Unit>.NotFound("Target event not found");
            }

            if (soldEvent.ItemSellCancelledEvent != null)
            {
                return HandlerResult<Unit>.Ok(Unit.New);
            }
            
            if(soldEvent.ItemDeliveredEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Item already delivered and cannot be cancelled");
            }

            var entity = new ItemCancelledEvent
            {
                InstanceSoldEventId = soldEvent.Id,
                Reason = request.Reason,
                RefundedAmount = request.RefundedAmount
            };

            _appDbContext.ItemCancelledEvents.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        public ValidationResult Validate(ItemCancelledEventDto request) => new Validator().Validate(request);

        private class Validator : AbstractValidator<ItemCancelledEventDto>
        {
            public Validator()
            {
                RuleFor(x => x.TargetEventId)
                    .NotEmpty()
                    .WithMessage("Target event not specified");

                RuleFor(x => x.Reason)
                    .NotEmpty()
                    .WithMessage("Cancellation must specify a reason");
            }
        }
    }
}
