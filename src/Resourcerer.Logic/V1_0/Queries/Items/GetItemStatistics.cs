﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1_0.Functions;

namespace Resourcerer.Logic.Queries.V1_0;
public static class GetItemStatistics
{
    public class Handler : IAppHandler<(Guid ItemId, DateTime Now), List<V1ItemStatistics>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<List<V1ItemStatistics>>> Handle((Guid ItemId, DateTime Now) query)
        {
            var itemQuery = _appDbContext.Items
                .Include(x => x.Category)
                .Include(x => x.UnitOfMeasure)
                .Include(x => x.Prices)
                .Include(x => x.ElementExcerpts)
                .Include(x => x.Instances)
                    .ThenInclude(x => x.SourceInstance)
                .Include(x => x.CompositeExcerpts)
                    .ThenInclude(x => x.Element)
                        .ThenInclude(x => x!.Prices) as IQueryable<Item>;
            
            var item = await itemQuery.FirstOrDefaultAsync(x => x.Id == query.ItemId);

            if (item == null)
            {
                return HandlerResult<List<V1ItemStatistics>>.Ok(new List<V1ItemStatistics>());
            }

            var instanceInfos = item.Instances
                .Select(x => Instances.GetInstanceInfo(x, query.Now))
                .ToArray();

            var isComposite = item.CompositeExcerpts.Any();

            var usedInComposites = item.ElementExcerpts.Count();

            var productionCostAsComposite = item.CompositeExcerpts
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

        public ValidationResult Validate((Guid ItemId, DateTime Now) request) => new Validator().Validate(request);

        private class Validator : AbstractValidator<(Guid ItemId, DateTime Now)>
        {
            public Validator()
            {
                RuleFor(x => x.ItemId).NotEmpty();
            }
        }
    }
}
