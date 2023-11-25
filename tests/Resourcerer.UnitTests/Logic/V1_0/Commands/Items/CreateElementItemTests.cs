using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateElementItemTests : TestsBase
{
    private readonly CreateElementItem.Handler _handler;
    public CreateElementItemTests()
    {
        _handler = new CreateElementItem.Handler(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var category = Mocker.MockCategory(_testDbContext);
        var uom = Mocker.MockUnitOfMeasure(_testDbContext);
        var dto = new CreateElementItemDto
        {
            Name = "test",
            CategoryId = category.Id,
            UnitOfMeasureId = uom.Id,
            UnitPrice = 2
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();
        var entity = _testDbContext.Items.First();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.Contains(_testDbContext.Items, x => x.Name == dto.Name);
    }

    [Fact]
    public void When_ElementWithSameName_Exsts_Then_ValidationError()
    {
        // arrange
        var existingElement = Mocker.MockItem(_testDbContext);
        var category = Mocker.MockCategory(_testDbContext);
        var uom = Mocker.MockUnitOfMeasure(_testDbContext);
        var dto = new CreateElementItemDto
        {
            Name = existingElement.Name,
            CategoryId = category.Id,
            UnitOfMeasureId = uom.Id,
            UnitPrice = 2
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_Category_NotFound_Then_ValidationError()
    {
        // arrange
        var uom = Mocker.MockUnitOfMeasure(_testDbContext);
        var dto = new CreateElementItemDto
        {
            Name = "test",
            CategoryId = Guid.NewGuid(),
            UnitOfMeasureId = uom.Id,
            UnitPrice = 2
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_UnitOfMeasure_NotFound_Then_ValidationError()
    {
        // arrange
        var category = Mocker.MockCategory(_testDbContext);
        var dto = new CreateElementItemDto
        {
            Name = "test",
            CategoryId = category.Id,
            UnitOfMeasureId = Guid.NewGuid(),
            UnitPrice = 2
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
