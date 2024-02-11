using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.V1_0;

public static class CreateItemOrderedEvent
{
    public class Handler : IAppHandler<ItemOrderedEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(ItemOrderedEventDto request)
        {
            var item = await _appDbContext.Items
                .Include(x => x.UnitOfMeasure)
                .FirstOrDefaultAsync(x => x.Id == request.ItemId);

            if (item == null)
            {
                return HandlerResult<Unit>
                    .Rejected($"Item with id {request.ItemId} not found");
            }

            if(item.ExpirationTimeSeconds != null)
            {
                if(request.ExpectedDeliveryDate == null)
                {
                    return HandlerResult<Unit>
                        .Rejected($"Expected delivery time must be set for items that can expire");
                }

                if (request.ExpiryDate == null)
                {
                    return HandlerResult<Unit>
                        .Rejected($"Expiry date must be set for items that can expire");
                }

                if(request.ExpectedDeliveryDate >= request.ExpiryDate)
                {
                    return HandlerResult<Unit>
                        .Rejected($"Ordered items will expire before they are delivered");
                }
            }

            var instance = new Instance
            {
                Id = Guid.NewGuid(),
                ExpiryDate = request.ExpiryDate,
                
                ItemId = item.Id
            };

            var orderedEvent = new InstanceOrderedEvent
            {
                UnitPrice = request.UnitPrice,
                Quantity = request.UnitsOrdered,
                TotalDiscountPercent = request.TotalDiscountPercent,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate,
            };

            _appDbContext.ItemOrderedEvents.Add(orderedEvent);
            _appDbContext.Instances.Add(instance);
            
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(ItemOrderedEventDto request) =>
            new Validator().Validate(request);

        private class Validator : AbstractValidator<ItemOrderedEventDto>
        {
            public Validator()
            {
                RuleFor(x => x.ItemId)
                    .NotEmpty()
                    .WithMessage("Item id cannot be empty");

                RuleFor(x => x.Seller)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Seller cannot be empty");

                RuleFor(x => x.Buyer)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Buyer cannot be empty");

                RuleFor(x => x.UnitsOrdered)
                    .GreaterThan(0)
                    .WithMessage("Number of ordered units must be greater than 0");

                RuleFor(x => x.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("Unit price must be greater than 0");

                RuleFor(x => x.TotalDiscountPercent)
                    .InclusiveBetween(0, 100)
                    .WithMessage("Total discount must be greater between in 0 and 100");
            }
        }
    }
}
