using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;

namespace Resourcerer.Logic.V1;

public class UpdateCompositeItem
{
    public class Handler : IAppHandler<V1ChangeCompositeItemRecipe, ItemDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validator;
        private readonly IMapper _mapper;

        public Handler(AppDbContext dbContext, Validator validator, IMapper mapper)
        {
            _dbContext = dbContext;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<HandlerResult<ItemDto>> Handle(V1ChangeCompositeItemRecipe request)
        {
            var composite = await _dbContext.Items
                .Include(x => x.Recipes)
                .Include(x => x.Prices)
                .FirstOrDefaultAsync(x => x.Id == request.ItemId);

            if (composite == null)
                return HandlerResult<ItemDto>.NotFound("Composite item not found");

            if(composite.Recipes.Count == 0)
                return HandlerResult<ItemDto>.Rejected($"Item is not of composite type");

            var categoryUpdateResult = await UpdateRelationAsync(
                () => composite.CategoryId == request.CategoryId,
                () => _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == request.CategoryId),
                (x) => composite.CategoryId = x.Id
            );
            
            if(categoryUpdateResult != eHandlerResultStatus.Ok)
                return HandlerResult<ItemDto>.Rejected($"Specified category not found");

            var uomUpdateResult = await UpdateRelationAsync(
                () => composite.UnitOfMeasureId == request.UnitOfMeasureId,
                () => _dbContext.UnitsOfMeasure.FirstOrDefaultAsync(x => x.Id == request.UnitOfMeasureId),
                (x) => composite.UnitOfMeasureId = x.Id
            );

            if (uomUpdateResult != eHandlerResultStatus.Ok)
                return HandlerResult<ItemDto>.Rejected($"Specified unit of measure not found");

            var recipeUpdateResult = await AddRecipeAsync(composite, request.ExcerptMap!);
            
            if(recipeUpdateResult != eHandlerResultStatus.Ok)
                return HandlerResult<ItemDto>.Rejected("Not all required items have been found");

            UpdatePrice(composite, request.UnitPrice);

            composite.Name = request.Name;
            composite.ExpirationTimeSeconds = request.ExpirationTimeSeconds;
            composite.ProductionPrice = request.ProductionPrice;
            composite.ProductionTimeSeconds = request.ProductionTimeSeconds;

            await _dbContext.SaveChangesAsync();

            var dto = _mapper.Map<ItemDto>(composite);

            return HandlerResult<ItemDto>.Ok(dto);
        }

        private async Task<eHandlerResultStatus> AddRecipeAsync(Item composite, Dictionary<Guid, double> excerptMap)
        {
            var latestRecipe = composite.Recipes
               .OrderByDescending(x => x.Version)
               .First();

            var requiredItemIds = excerptMap!.Keys.ToArray();

            var items = await _dbContext.Items
                .Where(x => requiredItemIds.Contains(x.Id))
                .Select(x => new Item
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToArrayAsync();

            if (requiredItemIds.Length > items.Length)
                return eHandlerResultStatus.Rejected;

            var recipe = new Recipe
            {
                CompositeItemId = composite.Id,
                Version = latestRecipe.Version + 1,
                RecipeExcerpts = items.Select(x => new RecipeExcerpt
                {
                    ElementId = x.Id,
                }).ToArray()
            };

            composite.Recipes.Add(recipe);
            return eHandlerResultStatus.Ok;
        }

        private void UpdatePrice(Item composite, double newPrice)
        {
            var lastPrice = composite.Prices.OrderBy(x => x.AuditRecord.CreatedAt).Last();
            if (lastPrice.UnitValue == newPrice) return;

            composite.Prices.Add(new Price
            {
                UnitValue = newPrice,
            });
        }

        private async Task<eHandlerResultStatus> UpdateRelationAsync<T>(
            Func<bool> idempotencyCheck,
            Func<Task<T?>> queryAsync,
            Action<T> updateWith) where T : class, IId<Guid>
        {
            if(idempotencyCheck()) return eHandlerResultStatus.Ok;

            var entity = await queryAsync();
            
            if(entity == null)
                return eHandlerResultStatus.Rejected;

            updateWith(entity);

            return eHandlerResultStatus.Ok;
        }

        public ValidationResult Validate(V1ChangeCompositeItemRecipe request) =>
            _validator.Validate(request);
    }

    public class Validator : AbstractValidator<V1ChangeCompositeItemRecipe>
    {
        public Validator()
        {
            RuleFor(x => x.ItemId)
                .NotEmpty()
                .WithMessage("Composite id cannot be empty");

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
