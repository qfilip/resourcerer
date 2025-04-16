using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Enums;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items;

public class GetItemTypeTests : TestsBase
{
    private readonly GetItemType.Handler _sut;
    public GetItemTypeTests()
    {
        _sut = new(_ctx);
    }

    [Fact]
    public void IsElement_Ok()
    {
        // arrange
        var item = _forger.Fake<Item>();
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(item.Id).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.Equal(eItemType.Element, result.Object!)
        );
    }

    [Fact]
    public void IsComposite_Ok()
    {
        // arrange
        var item = _forger.Fake<Item>();
        var recipe = _forger.Fake<Recipe>(x => x.CompositeItem = item);
        var excerpts = Enumerable.Range(0, 3)
            .Select(_ => _forger.Fake<RecipeExcerpt>(x => x.Recipe = recipe))
            .ToArray();

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(item.Id).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.Equal(eItemType.Composite, result.Object!)
        );
    }

    [Fact]
    public void NotFound_NotFound()
    {
        // arrange
        var item = _forger.Fake<Item>();
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(Guid.NewGuid()).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }
}
