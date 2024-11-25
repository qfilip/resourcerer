using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1.Items;
public static class GetItemStatistics
{
    public class Handler : IAppHandler<(Guid ItemId, DateTime Now), List<V1ItemStatistics>>
    {
        private readonly AppDbContext _appDbContext;
        private readonly Validator _validator;

        public Handler(AppDbContext appDbContext, Validator validator)
        {
            _appDbContext = appDbContext;
            _validator = validator;
        }

        public async Task<HandlerResult<List<V1ItemStatistics>>> Handle((Guid ItemId, DateTime Now) query)
        {
            var itemQuery = _appDbContext.Items
                .Include(x => x.Category)
                .Include(x => x.UnitOfMeasure)
                .Include(x => x.Prices)
                .Include(x => x.ElementRecipeExcerpts)
                .Include(x => x.Instances)
                    .ThenInclude(x => x.SourceInstance)
                .Include(x => x.Recipes)
                    .ThenInclude(x => x.RecipeExcerpts) as IQueryable<Item>;

            var item = await itemQuery.FirstOrDefaultAsync(x => x.Id == query.ItemId);

            if (item == null)
            {
                return HandlerResult<List<V1ItemStatistics>>.Ok(new List<V1ItemStatistics>());
            }

            var instanceInfos = item.Instances
                .Select(x => Functions.Instances.GetInstanceInfo(x, query.Now))
                .ToArray();

            var isComposite = item.Recipes.Any();

            var usedInComposites = item.ElementRecipeExcerpts.Count();

            var productionCostAsComposite = item.Recipes
                .OrderByDescending(x => x.Version)
                .First()
                .RecipeExcerpts
                .SelectMany(x => x.Element!.Prices)
                .Sum(x => x.UnitValue);

            var sellingCost = item.Prices.Single().UnitValue;

            var result = new V1ItemStockInfo
            {
                Id = item.Id,
                Name = item.Name,
                TotalUnitsInStock = instanceInfos.Sum(x => x.QuantityLeft),
                InstancesInfo = instanceInfos,
                ItemType = isComposite ? new[] { "Element", "Composite" } : new[] { "Element" },
                ProductionCostAsComposite = productionCostAsComposite,
                SellingPrice = item.Prices.Single().UnitValue
            };

            return HandlerResult<List<V1ItemStatistics>>.Ok(new List<V1ItemStatistics>());
        }

        public ValidationResult Validate((Guid ItemId, DateTime Now) request) => _validator.Validate(request);
    }
    public class Validator : AbstractValidator<(Guid ItemId, DateTime Now)>
    {
        public Validator()
        {
            RuleFor(x => x.ItemId).NotEmpty();
        }
    }
}
