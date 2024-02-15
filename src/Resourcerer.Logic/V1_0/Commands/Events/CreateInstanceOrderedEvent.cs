using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos;
using Resourcerer.Logic.Functions.V1_0;

namespace Resourcerer.Logic.Commands.V1_0;

public static class CreateInstanceOrderedEvent
{
    public class Handler : IAppHandler<InstanceOrderRequestDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(InstanceOrderRequestDto request)
        {
            var companies = await _appDbContext.Companies
                .Where(x =>
                    x.Id == request.BuyerCompanyId ||
                    x.Id == request.SellerCompanyId)
                .ToArrayAsync();

            var errors = new List<string>();
            
            if (companies.Any(x => x.Id == request.BuyerCompanyId))
            {
                errors.Add($"Buyer with id {request.BuyerCompanyId} not found");
            }

            if (companies.Any(x => x.Id == request.SellerCompanyId))
            {
                errors.Add($"Seller with id {request.SellerCompanyId} not found");
            }

            if(errors.Count > 0)
            {
                return HandlerResult<Unit>.Rejected(errors);
            }

            var instance = await _appDbContext.Instances
                .Include(x => x.Item)
                .Include(x => x.SourceInstance)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.InstanceId &&
                    x.OwnerCompanyId == request.SellerCompanyId);

            if (instance == null)
            {
                return HandlerResult<Unit>
                    .Rejected($"Instance with id {request.InstanceId} not found");
            }

            if (instance.ExpiryDate != null)
            {
                errors.Clear();

                if (request.ExpectedDeliveryDate == null)
                {
                    errors.Add($"Expected delivery time must be set for instances that can expire");
                }

                if(request.ExpectedDeliveryDate >= instance.ExpiryDate)
                {
                    errors.Add($"Ordered instances will expire before they are delivered");
                }

                if (errors.Count > 0)
                {
                    return HandlerResult<Unit>.Rejected(errors);
                }
            }

            var unitsSent = instance.OrderedEvents
                .Where(x =>
                    x.OrderCancelledEvent == null &&
                    x.SentEvent != null)
                .Sum(x => x.Quantity);

            var unitsInStock = Instances.GetUnitsInStock(instance) - unitsSent;

            if(unitsInStock - request.UnitsOrdered <= 0)
            {
                return HandlerResult<Unit>
                    .Rejected($"Not enough units left in stock for this instance");
            }

            var orderedEvent = JsonEntityBase.CreateEntity(() =>
            {
                return new InstanceOrderedEvent
                {
                    DerivedInstanceId = Guid.NewGuid(),
                    BuyerCompanyId = request.BuyerCompanyId,
                    SellerCompanyId = request.SellerCompanyId,
                    
                    UnitPrice = request.UnitPrice,
                    Quantity = request.UnitsOrdered,
                    TotalDiscountPercent = request.TotalDiscountPercent,
                    ExpectedDeliveryDate = request.ExpectedDeliveryDate
                };
            });

            instance.OrderedEvents.Add(orderedEvent);
            
            var newOwnerInstance = new Instance
            {
                OwnerCompanyId = orderedEvent.BuyerCompanyId,
                SourceInstanceId = instance.Id,
                ItemId = instance.ItemId,
                ExpiryDate = instance.ExpiryDate
            };

            _appDbContext.Instances.Add(newOwnerInstance);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(InstanceOrderRequestDto request) =>
            new Validator().Validate(request);

        public static ValidationResult ValidateRequest(InstanceOrderRequestDto request) =>
            new Validator().Validate(request);

        private class Validator : AbstractValidator<InstanceOrderRequestDto>
        {
            public Validator()
            {
                RuleFor(x => x.InstanceId)
                    .NotEmpty()
                    .WithMessage("instance id cannot be empty");

                RuleFor(x => x.SellerCompanyId)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Seller cannot be empty");

                RuleFor(x => x.BuyerCompanyId)
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
