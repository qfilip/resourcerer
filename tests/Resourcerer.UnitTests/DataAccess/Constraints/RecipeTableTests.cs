using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Logic;

namespace Resourcerer.UnitTests.DataAccess.Constraints;

public class RecipeTableTests : TestsBase
{
    [Fact]
    public void UniqueConstraintCheck__ValidData()
    {
        // arrange
        var item1 = _forger.Fake<Item>();
        var item2 = _forger.Fake<Item>();
        
        var recipes1 = Enumerable.Range(0, 2)
            .Select(i => _forger.Fake<Recipe>(x =>
            {
                x.CompositeItem = item1;
                x.Version = i;
            }))
            .ToArray();

        var recipes2 = Enumerable.Range(0, 2)
            .Select(i => _forger.Fake<Recipe>(x =>
            {
                x.CompositeItem = item2;
                x.Version = i;
            }))
            .ToArray();

        // act, assert
        _ctx.SaveChanges();
    }

    [Fact]
    public void UniqueConstraintCheck__InvalidData()
    {
        // arrange
        var item = _forger.Fake<Item>();

        var recipes = Enumerable.Range(0, 2)
            .Select(i => _forger.Fake<Recipe>(x =>
            {
                x.CompositeItem = item;
                x.Version = 1;
            }))
            .ToArray();

        // act, assert
        Assert.Throws<DbUpdateException>(() => _ctx.SaveChanges());
    }
}
