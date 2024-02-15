using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateCategoryTests : TestsBase
{
    private readonly CreateCategory.Handler _handler;
    public CreateCategoryTests()
    {
        _handler = new CreateCategory.Handler(_testDbContext);
    }

    [Fact]
    public void When_HappyPath_TopCategory_Then_Ok()
    {
        var dto = new CreateCategoryDto
        {
            Name = "name",
            CompanyId = DF.FakeCompany(_testDbContext).Id,
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_HappyPath_ChildCategory_Then_Ok()
    {
        var parentCategory = DF.FakeCategory(_testDbContext, x =>
        {
            x.CompanyId = DF.FakeCompany(_testDbContext).Id;
        });
        var dto = new CreateCategoryDto
        {
            Name = "name",
            CompanyId = parentCategory.CompanyId,
            ParentCategoryId = parentCategory.Id
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_Categories_DifferentCompany_And_SameParentCategory_HaveSameNamee_Then_Ok()
    {
        var c = DF.FakeCategory(_testDbContext);
        var dto = new CreateCategoryDto
        {
            Name = c.Name,
            CompanyId = DF.FakeCompany(_testDbContext).Id
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_SameCompany_And_DifferentParentCategory_HaveSameName_Then_Ok()
    {
        var parentCatg = DF.FakeCategory(_testDbContext);
        var dto = new CreateCategoryDto
        {
            Name = parentCatg.Name,
            CompanyId = parentCatg.CompanyId,
            ParentCategoryId = parentCatg.Id
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_SameCompany_And_SameParentCategory_HaveSameName_Then_Rejected()
    {
        var parentCatg = DF.FakeCategory(_testDbContext);
        var existingCatg = DF.FakeCategory(_testDbContext, x =>
        {
            x.CompanyId = parentCatg.CompanyId;
            x.ParentCategoryId = parentCatg.Id;
        });
        var dto = new CreateCategoryDto
        {
            Name = existingCatg.Name,
            CompanyId = existingCatg.Id,
            ParentCategoryId = parentCatg.Id
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_ParentCategory_DoesntExist_Then_Rejected()
    {
        // arrange
        var dto = new CreateCategoryDto
        {
            Name = "test",
            CompanyId = DF.FakeCompany(_testDbContext).Id,
            ParentCategoryId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_ParentCategory_WithDifferentCompany_Exist_Then_Rejected()
    {
        // arrange
        var parentCatg = DF.FakeCategory(_testDbContext);
        var dto = new CreateCategoryDto
        {
            Name = "test",
            CompanyId = DF.FakeCompany(_testDbContext).Id,
            ParentCategoryId = parentCatg.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
