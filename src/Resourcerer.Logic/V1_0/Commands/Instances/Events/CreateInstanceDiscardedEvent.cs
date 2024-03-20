using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;

using QU = Resourcerer.DataAccess.Utilities.Query.Instances;

namespace Resourcerer.Logic.V1_0.Commands;

public static class CreateInstanceDiscardedEvent
{
    public class Handler : IAppEventHandler<V1InstanceDiscardedRequest, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1InstanceDiscardedRequest request)
        {
            var instance = await _appDbContext.Instances
                .Select(QU.Expand(x => new Instance
                {
                    OrderedEventsJson = x.OrderedEventsJson,
                    DiscardedEventsJson = x.DiscardedEventsJson
                }))
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.InstanceId);

            if (instance == null)
            {
                return HandlerResult<Unit>.NotFound($"Instance with id {request.InstanceId} not found");
            }

            var sent = instance.OrderedEvents
                .Where(x =>
                    x.OrderCancelledEvent == null &&
                    x.SentEvent != null)
                .Sum(x => x.Quantity);

            var discarded = instance.DiscardedEvents
                .Sum(x => x.Quantity);

            var quantityLeft = instance.Quantity - sent - discarded;

            if (quantityLeft < 0)
            {
                var error = $"More instances spent than ordered for instance id: {instance!.Id}";
                throw new DataCorruptionException(error);
            }
            else if (quantityLeft == 0)
            {
                return HandlerResult<Unit>.Rejected("Remaining quantity is 0. Nothing to discard.");
            }
            else if (quantityLeft - request.Quantity < 0)
            {
                return HandlerResult<Unit>.Rejected($"Remaining quantity is {quantityLeft}. Cannot discard {request.Quantity} number of items");
            }
            else
            {
                var discardEvent = JsonEntityBase.CreateEntity(() =>
                {
                    return new InstanceDiscardedEvent
                    {
                        Quantity = request.Quantity,
                        Reason = request.Reason
                    };
                });

                instance.DiscardedEvents.Add(discardEvent);

                _appDbContext.Instances.Attach(instance);
                _appDbContext.Instances.Entry(instance).Property(x => x.DiscardedEventsJson).IsModified = true;

                await _appDbContext.SaveChangesAsync();

                return HandlerResult<Unit>.Ok(Unit.New);
            }
        }

        public static ValidationResult Validate(V1InstanceDiscardedRequest request) =>
            new Validator().Validate(request);

        private class Validator : AbstractValidator<V1InstanceDiscardedRequest>
        {
            public Validator()
            {
                RuleFor(x => x.InstanceId)
                    .NotEmpty()
                    .WithMessage("Instance id cannot be empty");

                RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be larger than 0");

                RuleFor(x => x.Reason)
                    .NotEmpty()
                    .WithMessage("Reason cannot be empty");

            }
        }
    }
}
