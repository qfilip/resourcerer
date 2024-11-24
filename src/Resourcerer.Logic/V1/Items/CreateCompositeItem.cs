using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1.Items;

public static class CreateCompositeItem
{
    public class Handler : IAppHandler<V1CreateCompositeItem, Unit>
    {
        private readonly AppDbContext _appDbContext;
        private readonly Validator _validatior;

        public Handler(AppDbContext appDbContext, Validator validatior)
        {
            _appDbContext = appDbContext;
            _validatior = validatior;
        }

        public async Task<HandlerResult<Unit>> Handle(V1CreateCompositeItem request)
        {
            var category = await _appDbContext.Categories
                .Select(x => new
                {
                    x.Id
                })
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if (category == null)
            {
                var error = "Requested category doesn't exist";
                return HandlerResult<Unit>.NotFound(error);
            }

            var existing = await _appDbContext.Items
                .FirstOrDefaultAsync(x =>
                    x.CategoryId == request.CategoryId &&
                    x.Name == request.Name);

            if (existing != null)
            {
                var error = "Element with the same name and category already exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var uom = await _appDbContext.UnitsOfMeasure
                .FirstOrDefaultAsync(x => x.Id == request.UnitOfMeasureId);

            if (uom == null)
            {
                var error = "Requested unit of measure doesn't exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var requiredElementIds = request.ExcerptMap!.Select(x => x.Key).ToArray();

            var requiredItems = await _appDbContext.Items
                .Where(x => requiredElementIds.Contains(x.Id))
                .ToArrayAsync();

            if (requiredItems.Length != requiredElementIds.Length)
            {
                var error = "All required elements don't exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var composite = new Item
            {
                Id = Guid.NewGuid(),

                Name = request.Name,
                ProductionTimeSeconds = request.PreparationTimeSeconds,
                ExpirationTimeSeconds = request.ExpirationTimeSeconds,

                CategoryId = category.Id,
                UnitOfMeasureId = uom.Id,
            };

            var price = new Price
            {
                ItemId = composite.Id,
                UnitValue = request.UnitPrice
            };

            var recipe = new Recipe()
            {
                Id = Guid.NewGuid(),
                CompositeItemId = composite.Id,
            };

            var excerpts = request.ExcerptMap!
                .Select(x => new RecipeExcerpt
                {
                    RecipeId = recipe.Id,
                    ElementId = x.Key,
                    Quantity = x.Value
                }).ToArray();

            _appDbContext.Items.Add(composite);
            _appDbContext.Prices.Add(price);
            _appDbContext.Recipes.Add(recipe);
            _appDbContext.Excerpts.AddRange(excerpts);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(V1CreateCompositeItem request) => _validatior.Validate(request);
    }
    public class Validator : AbstractValidator<V1CreateCompositeItem>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .Must(Validation.Item.Name)
                .WithMessage(Validation.Item.NameError);

            RuleFor(x => x.PreparationTimeSeconds)
                .Must(Validation.Item.ProductionTimeSeconds)
                .WithMessage(Validation.Item.ProductionTimeSecondsError);

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
                .Must(Validation.Item.Price)
                .WithMessage(Validation.Item.PriceError);

            RuleFor(x => x.ExcerptMap)
                .Must(x =>
                {
                    if (x == null) return false;
                    else return x.All(kv => kv.Key != Guid.Empty && kv.Value >= 0);
                })
                .WithMessage("Excerpt map contains invalid data");
        }
    }
}
