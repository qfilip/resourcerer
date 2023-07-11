using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Items;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.Commands.Items;

public class CreateItemTests : TestsBase
{
    private readonly CreateItem.Handler _handler;
    public CreateItemTests()
    {
        _handler = new CreateItem.Handler(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var category = Mocker.MockCategory(_testDbContext);
        var uom = Mocker.MockUnitOfMeasure(_testDbContext);
        var dto = new CreateItemDto
        {
            Name = "test",
            CategoryId = category.Id,
            UnitOfMeasureId = uom.Id,
            UnitPrice = 2
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();
        var entity = _testDbContext.Items.First();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.Equal(dto.Name, entity.Name);
    }

    [Fact]
    public void When_ElementWithSameName_Exsts_Then_ValidationError()
    {
        // arrange
        var existingElement = Mocker.MockItem(_testDbContext);
        var category = Mocker.MockCategory(_testDbContext);
        var uom = Mocker.MockUnitOfMeasure(_testDbContext);
        var dto = new CreateItemDto
        {
            Name = existingElement.Name,
            CategoryId = category.Id,
            UnitOfMeasureId = uom.Id,
            UnitPrice = 2
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void When_Category_NotFound_Then_ValidationError()
    {
        // arrange
        var uom = Mocker.MockUnitOfMeasure(_testDbContext);
        var dto = new CreateItemDto
        {
            Name = "test",
            CategoryId = Guid.NewGuid(),
            UnitOfMeasureId = uom.Id,
            UnitPrice = 2
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void When_UnitOfMeasure_NotFound_Then_ValidationError()
    {
        // arrange
        var category = Mocker.MockCategory(_testDbContext);
        var dto = new CreateItemDto
        {
            Name = "test",
            CategoryId = category.Id,
            UnitOfMeasureId = Guid.NewGuid(),
            UnitPrice = 2
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }
}
