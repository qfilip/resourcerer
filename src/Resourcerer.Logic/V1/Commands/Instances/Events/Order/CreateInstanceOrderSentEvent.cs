using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1.Commands;

public static class CreateInstanceOrderSentEvent
{
    public class Handler : IAppEventHandler<V1InstanceOrderSentRequest, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(V1InstanceOrderSentRequest request)
        {
            var instance = await _appDbContext.Instances
                .Select(x => new Instance
                {
                    Id = x.Id,
                    OrderedEvents = x.OrderedEvents
                        .Where(e => e.Id == request.OrderEventId)
                        .ToArray()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.InstanceId);

            if (instance == null)
            {
                return HandlerResult<Unit>.Rejected("Instance not found");
            }

            var orderEv = instance.OrderedEvents
                .FirstOrDefault(x => x.Id == request.OrderEventId);

            if (orderEv == null)
            {
                return HandlerResult<Unit>.Rejected("Order not found");
            }

            if(orderEv.CancelledEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Order was cancelled");
            }

            if (orderEv.DeliveredEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Order was delivered");
            }

            if (orderEv.SentEvent != null)
            {
                return HandlerResult<Unit>.Ok(Unit.New);
            }

            _appDbContext.InstanceOrderedEvents.Attach(orderEv);
            orderEv.SentEvent = AppDbJsonField.Create(() => new InstanceOrderSentEvent());

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        public static ValidationResult Validate(V1InstanceOrderSentRequest request) =>
           new Validator().Validate(request);

        public class Validator : AbstractValidator<V1InstanceOrderSentRequest>
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
            }
        }
    }
}
