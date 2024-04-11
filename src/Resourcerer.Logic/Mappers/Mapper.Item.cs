using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static Item Map(ItemDto src) =>
        Map(() =>
            new Item
            {
                Name = src.Name,
                ProductionPrice = src.ProductionPrice,
                ProductionTimeSeconds = src.ProductionTimeSeconds,
                ExpirationTimeSeconds = src.ExpirationTimeSeconds,

                CategoryId = src.CategoryId,
                Category = Map(src.Category, Map),

                UnitOfMeasureId = src.UnitOfMeasureId,
                UnitOfMeasure = Map(src.UnitOfMeasure, Map),

                ElementExcerpts = src.ElementExcerpts.Select(Map).ToArray(),
                CompositeExcerpts = src.CompositeExcerpts.Select(Map).ToArray(),
                Prices = src.Prices.Select(Map).ToArray(),
                Instances = src.Instances.Select(Map).ToArray(),
                ProductionOrders = src.ProductionOrders.Select(Map).ToArray(),
            }, src);

    public static ItemDto Map(Item src) =>
        Map(() =>
            new ItemDto
            {
                Name = src.Name,
                ProductionPrice = src.ProductionPrice,
                ProductionTimeSeconds = src.ProductionTimeSeconds,
                ExpirationTimeSeconds = src.ExpirationTimeSeconds,

                CategoryId = src.CategoryId,
                Category = Map(src.Category, Map),

                UnitOfMeasureId = src.UnitOfMeasureId,
                UnitOfMeasure = Map(src.UnitOfMeasure, Map),

                ElementExcerpts = src.ElementExcerpts.Select(Map).ToArray(),
                CompositeExcerpts = src.CompositeExcerpts.Select(Map).ToArray(),
                Prices = src.Prices.Select(Map).ToArray(),
                Instances = src.Instances.Select(Map).ToArray(),
                ProductionOrders = src.ProductionOrders.Select(Map).ToArray(),
            }, src);
}
