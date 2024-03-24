using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1.Commands.Items;

public class CreateElementItemTests : TestsBase
{
    private readonly CreateElementItem.Handler _handler;
    public CreateElementItemTests()
    {
        _handler = new CreateElementItem.Handler(_ctx);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var category = DF.Fake<Category>(_ctx);
        var uom = DF.Fake<UnitOfMeasure>(_ctx);
        var dto = new V1CreateElementItem
        {
            Name = "test",
            CompanyId = category.CompanyId,
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
        var existingElement = DF.Fake<Item>(_ctx);
        var dto = new V1CreateElementItem
        {
            Name = existingElement.Name,
            CompanyId = existingElement.Category!.CompanyId,
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
        var comp = DF.Fake<Company>(_ctx);
        var uom = DF.Fake<UnitOfMeasure>(_ctx);
        var dto = new V1CreateElementItem
        {
            Name = "test",
            CompanyId = comp.Id,
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
    public void Company_NotFound__Rejected()
    {
        // arrange
        var catg = DF.Fake<Category>(_ctx);
        var uom = DF.Fake<UnitOfMeasure>(_ctx);
        var dto = new V1CreateElementItem
        {
            Name = "test",
            CompanyId = Guid.NewGuid(),
            CategoryId = catg.Id,
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
        var category = DF.Fake<Category>(_ctx);
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
