using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items;

public class ChangeItemCategoryTests : TestsBase
{
    private readonly ChangeItemCategory.Handler _sut;
    public ChangeItemCategoryTests()
    {
        _sut = new(_ctx, new());
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var item = _forger.Fake<Item>();
        var newCategory = _forger.Fake<Category>();

        var dto = new V1ChangeItemCategory
        {
            ItemId = item.Id,
            NewCategoryId = newCategory.Id,
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        //assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                var entity = _ctx.Items.First(x => x.Id == dto.ItemId);
                Assert.Equal(dto.NewCategoryId, entity.CategoryId);
            }
        );
    }

    [Fact]
    public void ItemNotFound__NotFound()
    {
        // arrange
        _ = _forger.Fake<Item>();
        var newCategory = _forger.Fake<Category>();

        var dto = new V1ChangeItemCategory
        {
            ItemId = Guid.NewGuid(),
            NewCategoryId = newCategory.Id,
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        //assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void CategoryNotFound__Rejected()
    {
        // arrange
        var item = _forger.Fake<Item>();
        _ = _forger.Fake<Category>();

        var dto = new V1ChangeItemCategory
        {
            ItemId = item.Id,
            NewCategoryId = Guid.NewGuid(),
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        //assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
