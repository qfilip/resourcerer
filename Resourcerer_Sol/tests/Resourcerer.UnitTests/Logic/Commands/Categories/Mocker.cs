using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Logic.Commands.Categories;

internal static class Mocker
{
    public static Category MockCategory(AppDbContext context)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "test"
        };

        context.Categories.Add(category);
        context.SaveChanges();

        return category;
    }
}
