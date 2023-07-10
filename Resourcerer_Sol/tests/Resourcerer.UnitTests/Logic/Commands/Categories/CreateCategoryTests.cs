using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Categories;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.Commands.Categories;

public class CreateCategoryTests
{
    private readonly AppDbContext _testDbContext;
    private readonly CreateCategory.Handler _handler;
    public CreateCategoryTests()
    {
        _testDbContext = new ContextCreator().GetTestDbContext();
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
        var result = _handler.Handle(dto).GetAwaiter().GetResult();
        var entities = _testDbContext.Categories.ToList();
        
        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.Single(entities);
    }

    [Fact]
    public void When_Category_WithSameName_Exist_Then_ValidationError()
    {
        // arrange
        var existing = Mocker.MockCategory(_testDbContext);
        _testDbContext.SaveChanges();
        var dto = new CategoryDto
        {
            Name = existing.Name
        };

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();
        var entities = _testDbContext.Categories.ToList();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
        Assert.Single(entities);
    }

    [Fact]
    public void When_AddingChildCategory_And_ParentCategory_Exist_Then_Ok()
    {
        // arrange
        var existing = Mocker.MockCategory(_testDbContext);
        _testDbContext.SaveChanges();
        var dto = new CategoryDto
        {
            ParentCategoryId = existing.Id,
            Name = "test-child"
        };

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();
        var entities = _testDbContext.Categories.ToList();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.Equal(2, entities.Count);
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
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }
}
