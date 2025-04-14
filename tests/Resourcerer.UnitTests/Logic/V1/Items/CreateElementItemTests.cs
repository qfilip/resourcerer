using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items;

public class CreateElementItemTests : TestsBase
{
    private readonly CreateElementItem.Handler _handler;
    public CreateElementItemTests()
    {
        _handler = new CreateElementItem.Handler(_ctx, _mapper, new());
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var category = _forger.Fake<Category>();
        var uom = _forger.Fake<UnitOfMeasure>();
        var dto = new V1CreateElementItem
        {
            Name = "test",
            CategoryId = category.Id,
            UnitOfMeasureId = uom.Id,
            UnitPrice = 2
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var entity = _ctx.Items.First();
                Assert.Contains(_ctx.Items, x => x.Name == dto.Name);

            }
        );
    }

    [Fact]
    public void ElementWithSameName_Exsts__Rejected()
    {
        // arrange
        var existingElement = _forger.Fake<Item>();
        var dto = new V1CreateElementItem
        {
            Name = existingElement.Name,
            CategoryId = existingElement.CategoryId,
            UnitOfMeasureId = existingElement.UnitOfMeasure!.Id,
            UnitPrice = 2
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void Category_NotFound__Rejected()
    {
        // arrange
        var comp = _forger.Fake<Company>();
        var uom = _forger.Fake<UnitOfMeasure>();
        var dto = new V1CreateElementItem
        {
            Name = "test",
            CategoryId = Guid.NewGuid(),
            UnitOfMeasureId = uom.Id,
            UnitPrice = 2
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void UnitOfMeasure_NotFound__Rejected()
    {
        // arrange
        var category = _forger.Fake<Category>();
        var dto = new V1CreateElementItem
        {
            Name = "test",
            CategoryId = category.Id,
            UnitOfMeasureId = Guid.NewGuid(),
            UnitPrice = 2
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
