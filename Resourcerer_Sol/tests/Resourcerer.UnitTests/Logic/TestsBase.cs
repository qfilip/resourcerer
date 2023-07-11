using FakeItEasy;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly AppDbContext _testDbContext;
    public TestsBase()
    {
        _testDbContext = new ContextCreator().GetTestDbContext();
    }

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();

    [Fact]
    public void Test()
    {
        var catg = Mocker.MockCategory(_testDbContext);
        var uom = Mocker.MockUnitOfMeasure(_testDbContext);
        var item1 = new Item
        {
            Id = Guid.NewGuid(),
            Name = "vodka",
            CategoryId = catg.Id,
            UnitOfMeasureId = uom.Id
        };
        var item2 = new Item
        {
            Id = Guid.NewGuid(),
            Name = "gin",
            CategoryId = catg.Id,
            UnitOfMeasureId = uom.Id
        };
        var cocktail = new Item
        {
            Id = Guid.NewGuid(),
            Name = "gv",
            CategoryId = catg.Id,
            UnitOfMeasureId = uom.Id
        };

        var excerpts = new Excerpt[]
        {
            new() { CompositeId = cocktail.Id, ElementId = item1.Id, Quantity = 1 },
            new() { CompositeId = cocktail.Id, ElementId = item2.Id, Quantity = 1 },
        };

        _testDbContext.Items.AddRange(item1, item2, cocktail);
        // _testDbContext.Composites.Add(composite);
        _testDbContext.Excerpts.AddRange(excerpts);
        _testDbContext.SaveChanges();
        //var ctx2  = new ContextCreator().GetTestDbContext();
        //ctx2.Excerpts.AddRange(excerpts);
        //ctx2.SaveChanges();

        var x = 0;
    }
}
