using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static Category MockCategory(AppDbContext context)
    {
        var id = Guid.NewGuid();
        var category = new Category
        {
            Id = id,
            Name = $"test-{id}"
        };

        context.Categories.Add(category);

        return category;
    }

    public static Item MockItem(AppDbContext context, Action<Item>? modifier = null, int priceCount = 3, bool pricesCorrupted = false)
    {
        var id = Guid.NewGuid();
        var item = new Item
        {
            Id = id,
            
            Name = $"test-{id}",
            ExpirationTimeSeconds = 1200,
            PreparationTimeSeconds = 10,
            
            CategoryId = MockCategory(context).Id,
            UnitOfMeasureId = MockUnitOfMeasure(context).Id,
        };

        MockPrices(context, x => x.ItemId = item.Id, priceCount, pricesCorrupted);

        modifier?.Invoke(item);

        context.Items.Add(item);

        return item;
    }

    public static UnitOfMeasure MockUnitOfMeasure(AppDbContext context)
    {
        var id = Guid.NewGuid();
        var uom = new UnitOfMeasure
        {
            Id = id,
            Name = $"test-{id}",
            Symbol = "test"
        };

        context.UnitsOfMeasure.Add(uom);

        return uom;
    }

    public static List<Price> MockPrices(AppDbContext context, Action<Price> entityIdModifier, int priceCount, bool pricesCorrupted)
    {
        if (priceCount < 0)
        {
            throw new ArgumentException($"priceCount cannot be a negative number");
        }

        var prices = Enumerable.Range(0, priceCount)
           .Select(x => new Price
           {
               Id = Guid.NewGuid(),
               UnitValue = 10,
               EntityStatus = x == 0 ? eEntityStatus.Active : eEntityStatus.Deleted
           }).ToList();

        prices.ForEach(entityIdModifier);

        if (pricesCorrupted)
            prices.ForEach(x => x.EntityStatus = eEntityStatus.Active);

        context.AddRange(prices);

        return prices;
    }
}
