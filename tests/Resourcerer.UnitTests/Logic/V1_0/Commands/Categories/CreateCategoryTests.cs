using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateCategoryTests : TestsBase
{
    private readonly CreateCategory.Handler _handler;
    public CreateCategoryTests()
    {
        _handler = new CreateCategory.Handler(_testDbContext);
    }

    [Fact]
    public void When_Category_WithSameName_NotExist_Then_Ok()
    {
        // arrange
        var dto = new CategoryDto
        {
            Name = "Test"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_Category_WithSameName_Exist_Then_ValidationError()
    {
        // arrange
        var existing = DF.FakeCategory(_testDbContext);
        _testDbContext.SaveChanges();
        var dto = new CategoryDto
        {
            Name = existing.Name
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_AddingChildCategory_And_ParentCategory_Exist_Then_Ok()
    {
        // arrange
        var existing = DF.FakeCategory(_testDbContext);
        _testDbContext.SaveChanges();
        var dto = new CategoryDto
        {
            ParentCategoryId = existing.Id,
            Name = "test-child"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_AddingChildCategory_And_ParentCategory_NotExist_Then_ValidationError()
    {
        // arrange
        var dto = new CategoryDto
        {
            ParentCategoryId = Guid.NewGuid(),
            Name = "test-child"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
