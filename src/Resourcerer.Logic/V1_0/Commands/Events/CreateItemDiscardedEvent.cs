using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic.Exceptions;

namespace Resourcerer.Logic.V1_0.Commands;

public static class CreateItemDiscardedEvent
{
    public class Handler : IAppHandler<ItemDiscardedEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(ItemDiscardedEventDto request)
        {
            var instance = await _appDbContext.Instances
                    .Include(x => x.ItemOrderedEvents)
                        .ThenInclude(x => x!.InstanceDeliveredEvent)
                    .Include(x => x.ItemOrderedEvents)
                        .ThenInclude(x => x!.InstanceOrderCancelledEvent)
                    .Include(x => x.ItemDiscardedEvents)
                .FirstOrDefaultAsync(x => x.Id == request.InstanceId);

            if (instance == null)
            {
                return HandlerResult<Unit>.NotFound($"Instance with id {request.InstanceId} not found");
            }

            var delivered = instance.ItemOrderedEvents
                .Where(x =>
                    x.Buyer == request.Owner &&
                    x.InstanceOrderCancelledEvent == null &&
                    x.InstanceDeliveredEvent != null)
                .Sum(x => x.Quantity);

            var sent = instance.ItemOrderedEvents
                .Where(x =>
                    x.Seller == request.Owner &&
                    x.InstanceOrderCancelledEvent == null &&
                    x.InstanceSentEvent != null)
                .Sum(x => x.Quantity);

            var discarded = instance.ItemDiscardedEvents
                .Where(x => x.Owner == request.Owner)
                .Sum(x => x.Quantity);

            var quantityLeft = delivered - sent - discarded;

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
                var entity = new InstanceDiscardedEvent
                {
                    InstanceId = instance!.Id,
                    Quantity = request.Quantity,
                    Reason = request.Reason
                };

                _appDbContext.ItemDiscardedEvents.Add(entity);
                await _appDbContext.SaveChangesAsync();

                return HandlerResult<Unit>.Ok(Unit.New);
            }
        }

        public ValidationResult Validate(ItemDiscardedEventDto request) =>
            new Validator().Validate(request);

        private class Validator : AbstractValidator<ItemDiscardedEventDto>
        {
            public Validator()
            {
                RuleFor(x => x.Owner)
                    .NotEmpty()
                    .WithMessage("Owner cannot be empty");

                RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be larger than 0");

                RuleFor(x => x.Reason)
                    .NotEmpty()
                    .WithMessage("Reason cannot be empty");

                RuleFor(x => x.InstanceId)
                    .NotEmpty()
                    .WithMessage("InstanceId cannot be empty");
            }
        }
    }
}
