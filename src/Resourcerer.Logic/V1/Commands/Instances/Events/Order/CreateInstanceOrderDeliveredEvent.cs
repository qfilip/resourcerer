using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using QU = Resourcerer.DataAccess.Utilities.Query.Instances;

namespace Resourcerer.Logic.Commands.V1;

public static class CreateInstanceOrderDeliveredEvent
{
    public class Handler : IAppEventHandler<V1InstanceOrderDeliveredRequest, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1InstanceOrderDeliveredRequest request)
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
                var message = $"Instance with id {request.InstanceId} not found";
                return HandlerResult<Unit>.Rejected(message);
            }

            var orderEvent = instance.OrderedEvents
                .FirstOrDefault(x => x.Id == request.OrderEventId);

            if (orderEvent == null)
            {
                var error = "Order event not found";
                return HandlerResult<Unit>.Rejected(error);
            }

            if(orderEvent.SentEvent == null)
            {
                var error = "Instance was not sent, so it cannot be delivered";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.OrderCancelledEvent != null)
            {
                var error = "Order was cancelled, so it cannot be delivered";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.DeliveredEvent != null)
            {
                return HandlerResult<Unit>.Ok(Unit.New);
            }

            orderEvent.DeliveredEvent = AppDbJsonField
                .Create(() => new InstanceOrderDeliveredEvent()); ;

            _appDbContext.Instances.Attach(instance);
            _appDbContext.Instances.Entry(instance).Property(x => x.OrderedEventsJson).IsModified = true;
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public static ValidationResult Validate(V1InstanceOrderDeliveredRequest request) =>
            new Validator().Validate(request);

        public class Validator : AbstractValidator<V1InstanceOrderDeliveredRequest>
        {
            public Validator()
            {
                RuleFor(x => x.InstanceId)
                    .NotEmpty()
                    .WithMessage("Instance id cannot be empty");

                RuleFor(x => x.OrderEventId)
                    .NotEmpty()
                    .WithMessage("Order event id cannot be empty");
            }
        }
    }
}
