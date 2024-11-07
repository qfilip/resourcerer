using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Categories;

public class CreateCategoryTests : TestsBase
{
    private readonly CreateCategory.Handler _handler;
    public CreateCategoryTests()
    {
        _handler = new CreateCategory.Handler(_ctx, new());
    }

    [Fact]
    public void HappyPath_TopCategory___Ok()
    {
        var dto = new V1CreateCategory
        {
            Name = "name",
            CompanyId = _forger.Fake<Company>().Id,
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void HappyPath_ChildCategory___Ok()
    {
        var parentCategory = _forger.Fake<Category>(x =>
        {
            x.Company = _forger.Fake<Company>();
        });
        var dto = new V1CreateCategory
        {
            Name = "name",
            CompanyId = parentCategory.Company!.Id,
            ParentCategoryId = parentCategory.Id
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void Categories_DifferentCompany_And_SameParentCategory_HaveSameNamee___Ok()
    {
        var c = _forger.Fake<Category>();
        var dto = new V1CreateCategory
        {
            Name = c.Name,
            CompanyId = _forger.Fake<Company>().Id
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void SameCompany_And_DifferentParentCategory_HaveSameName___Ok()
    {
        var parentCatg = _forger.Fake<Category>();
        var dto = new V1CreateCategory
        {
            Name = parentCatg.Name,
            CompanyId = parentCatg.Company!.Id,
            ParentCategoryId = parentCatg.Id
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void SameCompany_And_SameParentCategory_HaveSameName___Rejected()
    {
        var parentCatg = _forger.Fake<Category>();
        var existingCatg = _forger.Fake<Category>(x =>
        {
            x.CompanyId = parentCatg.Company!.Id;
            x.ParentCategoryId = parentCatg.Id;
        });
        var dto = new V1CreateCategory
        {
            Name = existingCatg.Name,
            CompanyId = existingCatg.Id,
            ParentCategoryId = parentCatg.Id
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void ParentCategory_DoesntExist___Rejected()
    {
        // arrange
        var dto = new V1CreateCategory
        {
            Name = "test",
            CompanyId = _forger.Fake<Company>().Id,
            ParentCategoryId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void ParentCategory_WithDifferentCompany_Exist___Rejected()
    {
        // arrange
        var parentCatg = _forger.Fake<Category>();
        var dto = new V1CreateCategory
        {
            Name = "test",
            CompanyId = _forger.Fake<Company>().Id,
            ParentCategoryId = parentCatg.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
