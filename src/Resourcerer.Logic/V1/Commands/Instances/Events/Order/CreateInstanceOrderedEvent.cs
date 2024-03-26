using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Functions;

namespace Resourcerer.Logic.V1.Commands;

public static class CreateInstanceOrderedEvent
{
    public class Handler : IAppEventHandler<V1InstanceOrderRequest, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(V1InstanceOrderRequest request)
        {
            var companies = await _appDbContext.Companies
                .Where(x =>
                    x.Id == request.BuyerCompanyId ||
                    x.Id == request.SellerCompanyId)
                .ToArrayAsync();

            var errors = new List<string>();
            
            if (!companies.Any(x => x.Id == request.BuyerCompanyId))
            {
                errors.Add($"Buyer with id {request.BuyerCompanyId} not found");
            }

            if (!companies.Any(x => x.Id == request.SellerCompanyId))
            {
                errors.Add($"Seller with id {request.SellerCompanyId} not found");
            }

            if(request.DerivedInstanceItemId.HasValue)
            {
                var derivedInstanceItem = await _appDbContext.Items
                    .Select(x => new
                    {
                        ItemId = x.Id,
                        x.Category!.CompanyId
                    })
                    .FirstOrDefaultAsync(x => x.ItemId == request.DerivedInstanceItemId);

                var itemExists =
                    derivedInstanceItem != null &&
                    derivedInstanceItem.CompanyId == request.BuyerCompanyId;

                if (!itemExists)
                {
                    return HandlerResult<Unit>.Rejected("Derived instance item not found");
                }
            }

            if(errors.Count > 0)
            {
                return HandlerResult<Unit>.Rejected(errors);
            }

            var instanceQuery = Instances.GetAvailableUnitsInStockDbQuery(_appDbContext.Instances);

            var instance = await instanceQuery
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.InstanceId);

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

            var availableUnitsInStock = Instances.GetAvailableUnitsInStock(instance);

            if(availableUnitsInStock - request.UnitsOrdered < 0)
            {
                return HandlerResult<Unit>
                    .Rejected($"Not enough units left in stock for this instance");
            }

            var orderedEvent = new InstanceOrderedEvent
            {
                InstanceId = instance.Id,
                DerivedInstanceId = Guid.NewGuid(),
                BuyerCompanyId = request.BuyerCompanyId,
                SellerCompanyId = request.SellerCompanyId,
                DerivedInstanceItemId = request.DerivedInstanceItemId,
                UnitPrice = request.UnitPrice,
                Quantity = request.UnitsOrdered,
                TotalDiscountPercent = request.TotalDiscountPercent,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate
            };
            
            var newOwnerInstance = new Instance
            {
                OwnerCompanyId = orderedEvent.BuyerCompanyId,
                SourceInstanceId = instance.Id,
                ItemId = request.DerivedInstanceItemId ?? instance.ItemId,
                ExpiryDate = instance.ExpiryDate
            };
            
            _appDbContext.InstanceOrderedEvents.Add(orderedEvent);
            _appDbContext.Instances.Add(newOwnerInstance);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
    public class Validator : AbstractValidator<V1InstanceOrderRequest>
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
