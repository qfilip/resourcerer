using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static Category MockCategory(AppDbContext context, Action<Category>? modifier = null)
    {
        var id = Guid.NewGuid();
        var category = new Category
        {
            Id = id,
            Name = $"test-{id}"
        };

        modifier?.Invoke(category);

        context.Categories.Add(category);

        return category;
    }

    public static Item MockItem(AppDbContext context, Action<Item>? modifier = null, double unitValue = 1, int priceCount = 3, bool pricesCorrupted = false)
    {
        var id = Guid.NewGuid();
        var item = new Item
        {
            Id = id,
            
            Name = $"test-{id}",
            ExpirationTimeSeconds = 1200,
            PreparationTimeSeconds = 10,
            
            Category = MockCategory(context),
            UnitOfMeasure = MockUnitOfMeasure(context),
            Prices = MockPrices(null, unitValue, priceCount, pricesCorrupted),
        };

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

    public static List<Excerpt> MockExcerpts(AppDbContext context, Item composite, (Item, double)[] itemsDetail)
    {
        var excerpts = new List<Excerpt>();
        foreach (var d in itemsDetail)
        {
            excerpts.Add(new Excerpt
            {
                CompositeId = composite.Id,
                ElementId = d.Item1.Id,
                Quantity = d.Item2
            });
        }

        context.Excerpts.AddRange(excerpts);

        return excerpts;
    }

    public static List<Price> MockPrices(Action<Price>? modifier, double unitValue, int priceCount, bool pricesCorrupted)
    {
        if (priceCount < 0)
        {
            throw new ArgumentException($"priceCount cannot be a negative number");
        }

        var prices = Enumerable.Range(0, priceCount)
           .Select(x => new Price
           {
               Id = Guid.NewGuid(),
               UnitValue = unitValue,
               EntityStatus = x == 0 ? eEntityStatus.Active : eEntityStatus.Deleted
           }).ToList();

        prices.ForEach(x => modifier?.Invoke(x));

        if (pricesCorrupted)
            prices.ForEach(x => x.EntityStatus = eEntityStatus.Active);

        return prices;
    }
}
