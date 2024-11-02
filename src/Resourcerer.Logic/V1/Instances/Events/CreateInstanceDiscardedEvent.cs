using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;

namespace Resourcerer.Logic.V1.Instances.Events;

public static class CreateInstanceDiscardedEvent
{
    public class Handler : IAppEventHandler<V1InstanceDiscardCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1InstanceDiscardCommand request)
        {
            var instance = await _appDbContext.Instances
                .Select(x => new
                {
                    x.Id,
                    x.Quantity,
                    x.OrderedEvents,
                    x.ReservedEvents,
                    x.DiscardedEvents
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.InstanceId);

            if (instance == null)
            {
                return HandlerResult<Unit>.NotFound($"Instance with id {request.InstanceId} not found");
            }

            var sent = instance.OrderedEvents
                .Where(x =>
                    x.CancelledEvent == null &&
                    x.SentEvent != null)
                .Sum(x => x.Quantity);

            var reserved = instance.ReservedEvents
                .Where(x => x.CancelledEvent == null)
                .Sum(x => x.Quantity);

            var discarded = instance.DiscardedEvents
                .Sum(x => x.Quantity);

            var quantityLeft = instance.Quantity - sent - reserved - discarded;

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
                var discardEvent = new InstanceDiscardedEvent
                {
                    InstanceId = instance.Id,
                    Quantity = request.Quantity,
                    Reason = request.Reason
                };

                _appDbContext.InstanceDiscardedEvents.Add(discardEvent);
                await _appDbContext.SaveChangesAsync();

                return HandlerResult<Unit>.Ok(Unit.New);
            }
        }
    }
    public class Validator : AbstractValidator<V1InstanceDiscardCommand>
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
