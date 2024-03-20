using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateCategoryTests : TestsBase
{
    private readonly CreateCategory.Handler _handler;
    public CreateCategoryTests()
    {
        _handler = new CreateCategory.Handler(_ctx);
    }

    [Fact]
    public void When_HappyPath_TopCategory_Then_Ok()
    {
        var dto = new V1CreateCategory
        {
            Name = "name",
            CompanyId = DF.FakeCompany(_ctx).Id,
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_HappyPath_ChildCategory_Then_Ok()
    {
        var parentCategory = DF.FakeCategory(_ctx, x =>
        {
            x.CompanyId = DF.FakeCompany(_ctx).Id;
        });
        var dto = new V1CreateCategory
        {
            Name = "name",
            CompanyId = parentCategory.CompanyId,
            ParentCategoryId = parentCategory.Id
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_Categories_DifferentCompany_And_SameParentCategory_HaveSameNamee_Then_Ok()
    {
        var c = DF.FakeCategory(_ctx);
        var dto = new V1CreateCategory
        {
            Name = c.Name,
            CompanyId = DF.FakeCompany(_ctx).Id
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_SameCompany_And_DifferentParentCategory_HaveSameName_Then_Ok()
    {
        var parentCatg = DF.FakeCategory(_ctx);
        var dto = new V1CreateCategory
        {
            Name = parentCatg.Name,
            CompanyId = parentCatg.CompanyId,
            ParentCategoryId = parentCatg.Id
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_SameCompany_And_SameParentCategory_HaveSameName_Then_Rejected()
    {
        var parentCatg = DF.FakeCategory(_ctx);
        var existingCatg = DF.FakeCategory(_ctx, x =>
        {
            x.CompanyId = parentCatg.CompanyId;
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
    public void When_ParentCategory_DoesntExist_Then_Rejected()
    {
        // arrange
        var dto = new V1CreateCategory
        {
            Name = "test",
            CompanyId = DF.FakeCompany(_ctx).Id,
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
        var parentCatg = DF.FakeCategory(_ctx);
        var dto = new V1CreateCategory
        {
            Name = "test",
            CompanyId = DF.FakeCompany(_ctx).Id,
            ParentCategoryId = parentCatg.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
