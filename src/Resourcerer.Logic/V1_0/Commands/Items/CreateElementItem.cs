using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.V1_0;

public static class CreateElementItem
{
    public class Handler : IAppHandler<CreateElementItemDto, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CreateElementItemDto request)
        {
            var existing = await _appDbContext.Items
                .FirstOrDefaultAsync(x => x.Name == request.Name);
            
            if(existing != null)
            {
                var error = "Element with the same name already exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var category = await _appDbContext.Categories
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if (category == null)
            {
                var error = "Requested category doesn't exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var uom = await _appDbContext.UnitsOfMeasure
                .FirstOrDefaultAsync(x => x.Id == request.UnitOfMeasureId);

            if (uom == null)
            {
                var error = "Requested unit of measure doesn't exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var item = new Item
            {
                Id = Guid.NewGuid(),
                ProductionTimeSeconds = request.PreparationTimeSeconds,
                ExpirationTimeSeconds = request.ExpirationTimeSeconds,
                Name = request.Name,
                CategoryId = request.CategoryId,
                UnitOfMeasureId = request.UnitOfMeasureId
            };

            var price = new Price
            {
                Item = item,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Items.Add(item);
            _appDbContext.Prices.Add(price);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(CreateElementItemDto request) => new Validator().Validate(request);

        private class Validator : AbstractValidator<CreateElementItemDto>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Element name cannot be empty")
                    .Length(min: 3, max: 50).WithMessage("Element name must be between 3 and 50 characters long");

                RuleFor(x => x.PreparationTimeSeconds)
                    .LessThan(0).WithMessage("PreparationTimeSeconds cannot be negative");

                RuleFor(x => x.ExpirationTimeSeconds)
                    .Must(x =>
                    {
                        if (x == null) return true;
                        else return x < 0;
                    }).WithMessage("ExpirationTimeSeconds cannot be negative");

                RuleFor(x => x.CategoryId)
                    .NotEmpty().WithMessage("Element's category cannot be empty");

                RuleFor(x => x.UnitOfMeasureId)
                    .NotEmpty().WithMessage("Element's unit of measure cannot be empty");

                RuleFor(x => x.UnitPrice)
                    .GreaterThan(0).WithMessage("Element's price must be greater than 0");
            }
        }
    }
}
