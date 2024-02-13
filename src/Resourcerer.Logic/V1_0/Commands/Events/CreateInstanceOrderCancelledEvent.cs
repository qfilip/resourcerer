using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.V1_0;

public static class CreateInstanceOrderCancelledEvent
{
    public class Handler : IAppHandler<InstanceCancelRequestDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(InstanceCancelRequestDto request)
        {
            var instance = await _appDbContext.Instances
                .FirstOrDefaultAsync(x => x.Id == request.InstanceId);

            if (instance == null)
            {
                var error = $"Instance with id {request.OrderEventId} not found";
                return HandlerResult<Unit>.Rejected(error);
            }

            var orderEvent = instance.OrderedEvents
                .FirstOrDefault(x => x.Id == request.OrderEventId);

            if (orderEvent == null)
            {
                var error = "Order event not found";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.SentEvent != null)
            {
                var error = "Order sent and cannot be cancelled";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.DeliveredEvent != null)
            {
                var error = "Order already delivered";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.OrderCancelledEvent != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var cancelEvent = JsonEntityBase.CreateEntity(() =>
            {
                return new InstanceOrderCancelledEvent
                {
                    Reason = request.Reason,
                    RefundedAmount = request.RefundedAmount
                };
            });

            orderEvent.OrderCancelledEvent = cancelEvent;

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(InstanceCancelRequestDto request) =>
            new Validator().Validate(request);

        public static ValidationResult ValidateRequest(InstanceCancelRequestDto request) =>
            new Validator().Validate(request);

        private class Validator : AbstractValidator<InstanceCancelRequestDto>
        {
            public Validator()
            {
                RuleFor(x => x.InstanceId)
                    .NotEmpty()
                    .WithMessage("Instance id cannot be empty");

                RuleFor(x => x.OrderEventId)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Order event id cannot be empty");

                RuleFor(x => x.Reason)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Reason cannot be empty");

                RuleFor(x => x.RefundedAmount)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Refunded Amount must be equal or greater than 0");
            }
        }
    }
}
