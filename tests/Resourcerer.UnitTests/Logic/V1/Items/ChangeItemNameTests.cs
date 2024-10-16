using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items;

public class ChangeItemNameTests : TestsBase
{
    private readonly ChangeItemName.Handler _sut;
    public ChangeItemNameTests()
    {
        _sut = new(_ctx, new(), GetMapster());
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var item = _forger.Fake<Item>();
        var dto = new V1ChangeItemName
        {
            ItemId = item.Id,
            NewName = "samsara"
        };
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var entity = _ctx.Items.First();

                Assert.Equal(dto.NewName, entity.Name);
                Assert.Equal(dto.NewName, result.Object!.Name);
            }
        );
    }

    [Fact]
    public void ItemNotFound__NotFound()
    {
        // arrange
        _forger.Fake<Item>();
        var dto = new V1ChangeItemName
        {
            ItemId = Guid.NewGuid(),
            NewName = "samsara"
        };
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }
}
