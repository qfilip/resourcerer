using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;
using Resourcerer.Logic.Models;

namespace Resourcerer.Logic.V1;

public class ChangeCompositeItemRecipe
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
                .FirstOrDefaultAsync(x => x.Id == request.CompositeId);

            if (composite == null)
                return HandlerResult<ItemDto>.NotFound("Composite item not found");

            if(composite.Recipes.Count == 0)
                throw new DataCorruptionException($"Recipe for composite item {composite.Id} not found");

            var latestRecipe = composite.Recipes
                .OrderByDescending(x => x.Version)
                .First();

            var requiredItemIds = request.ExcerptMap!.Keys.ToArray();

            var items = await _dbContext.Items
                .Where(x => requiredItemIds.Contains(x.Id))
                .Select(x => new Item
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToArrayAsync();

            if(requiredItemIds.Length > items.Length)
                return HandlerResult<ItemDto>.Rejected("Not all required items have been found");

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
            await _dbContext.SaveChangesAsync();

            var dto = _mapper.Map<ItemDto>(composite);

            return HandlerResult<ItemDto>.Ok(dto);
        }

        public ValidationResult Validate(V1ChangeCompositeItemRecipe request) =>
            _validator.Validate(request);
    }

    public class Validator : AbstractValidator<V1ChangeCompositeItemRecipe>
    {
        public Validator()
        {
            RuleFor(x => x.CompositeId)
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
