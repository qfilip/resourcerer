using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using QU = Resourcerer.DataAccess.Utilities.Query.Instances;

namespace Resourcerer.Logic.Commands.V1;

public static class CreateInstanceOrderCancelledEvent
{
    public class Handler : IAppEventHandler<V1InstanceOrderCancelRequest, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(V1InstanceOrderCancelRequest request)
        {
            var instance = await _appDbContext.Instances
                .Select(QU.Expand(x => new Instance
                {
                    OrderedEventsJson = x.OrderedEventsJson
                }))
                .AsNoTracking()
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

            _appDbContext.Instances.Attach(instance);
            _appDbContext.Instances.Entry(instance).Property(x => x.OrderedEventsJson).IsModified = true;
            
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public static ValidationResult Validate(V1InstanceOrderCancelRequest request) =>
            new Validator().Validate(request);

        private class Validator : AbstractValidator<V1InstanceOrderCancelRequest>
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
