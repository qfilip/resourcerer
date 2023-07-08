using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;

namespace Resourcerer.UnitTests.Logic.Commands.Composites;

internal static class Mocker
{
    public static Composite MockComposite(AppDbContext context)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "test"
        };
        var composite = new Composite
        {
            Id = Guid.NewGuid(),
            Name = "test",
            CategoryId = category.Id
        };
        var prices = Enumerable.Range(0, 5)
            .Select(x => new Price
            {
                Id = Guid.NewGuid(),
                CompositeId = composite.Id,
                EntityStatus = x == 0 ? eEntityStatus.Active : eEntityStatus.Deleted
            });

        context.Categories.Add(category);
        context.Composites.Add(composite);
        context.Prices.AddRange(prices);

        context.SaveChanges();

        return composite;
    }
}
