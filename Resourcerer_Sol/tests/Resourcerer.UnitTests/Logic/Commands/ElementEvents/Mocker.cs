using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Logic.Commands.ElementEvents;

internal class Mocker
{
    internal static ElementPurchasedEvent MockElementPurchasedEvent(AppDbContext ctx)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "test"
        };
        var uom = new UnitOfMeasure
        {
            Id = Guid.NewGuid(),
            Name = "test",
            Symbol = "t"
        };
        var element = new Element
        {
            Id = Guid.NewGuid(),
            Name = "test",
            CategoryId = category.Id,
            UnitOfMeasureId = uom.Id
        };
        
        ctx.Categories.Add(category);
        ctx.UnitsOfMeasure.Add(uom);
        ctx.Elements.Add(element);
        
        ctx.SaveChanges();

        return new()
        {
            Id = Guid.NewGuid(),
            ElementId = element.Id
        };
    }
}
