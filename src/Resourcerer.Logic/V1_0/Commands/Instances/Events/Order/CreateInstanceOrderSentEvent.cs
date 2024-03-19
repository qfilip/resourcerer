using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.Instances.Events.Order;

using QU = Resourcerer.DataAccess.Utilities.Query.Instances;

namespace Resourcerer.Logic.V1_0.Commands;

public static class CreateInstanceOrderSentEvent
{
    public class Handler : IAppEventHandler<InstanceOrderSentRequestDto, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(InstanceOrderSentRequestDto request)
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
                return HandlerResult<Unit>.Rejected("Instance not found");
            }

            var orderEv = instance.OrderedEvents
                .FirstOrDefault(x => x.Id == request.OrderEventId);

            if (orderEv == null)
            {
                return HandlerResult<Unit>.Rejected("Order not found");
            }

            if(orderEv.OrderCancelledEvent != null)
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

            orderEv.SentEvent = JsonEntityBase.CreateEntity(() => new InstanceOrderSentEvent());

            _appDbContext.Update(instance);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        public static ValidationResult Validate(InstanceOrderSentRequestDto request) =>
           new Validator().Validate(request);

        public class Validator : AbstractValidator<InstanceOrderSentRequestDto>
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
