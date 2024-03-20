using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateElementItemTests : TestsBase
{
    private readonly CreateElementItem.Handler _handler;
    public CreateElementItemTests()
    {
        _handler = new CreateElementItem.Handler(_ctx);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var category = DF.FakeCategory(_ctx);
        var uom = DF.FakeUnitOfMeasure(_ctx);
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
        var entity = _ctx.Items.First();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.Contains(_ctx.Items, x => x.Name == dto.Name);
    }

    [Fact]
    public void When_ElementWithSameName_Exsts_Then_ValidationError()
    {
        // arrange
        var existingElement = DF.FakeItem(_ctx);
        var dto = new V1CreateElementItem
        {
            Name = existingElement.Name,
            CompanyId = existingElement.Category!.CompanyId,
            CategoryId = existingElement.CategoryId,
            UnitOfMeasureId = existingElement.UnitOfMeasureId,
            UnitPrice = 2
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_Category_NotFound_Then_ValidationError()
    {
        // arrange
        var comp = DF.FakeCompany(_ctx);
        var uom = DF.FakeUnitOfMeasure(_ctx);
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
    public void When_Company_NotFound_Then_ValidationError()
    {
        // arrange
        var catg = DF.FakeCategory(_ctx);
        var uom = DF.FakeUnitOfMeasure(_ctx);
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
    public void When_UnitOfMeasure_NotFound_Then_ValidationError()
    {
        // arrange
        var category = DF.FakeCategory(_ctx);
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
