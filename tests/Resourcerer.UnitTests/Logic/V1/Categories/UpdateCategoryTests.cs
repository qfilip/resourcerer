﻿using FakeItEasy;
using Resourcerer.Application.Auth.Abstractions;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Categories;

public class UpdateCategoryTests : TestsBase
{
    private readonly UpdateCategory.Handler _sut;
    private readonly IAppIdentityService<AppUser> _fakeIdentityService = A.Fake<IAppIdentityService<AppUser>>();

    public UpdateCategoryTests()
    {
        _sut = new(_ctx, new(), _fakeIdentityService);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var company = FakeCompany();
        MockIdentity(company);

        var dto = new V1UpdateCategory
        {
            CategoryId = company.Categories.First().Id,
            NewParentCategoryId = company.Categories.Skip(1).Take(1).First().Id,
            NewName = "aryabhata"
        };
        
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var entity = _ctx.Categories.First(x => x.Id == dto.CategoryId);
                Assert.Equal(dto.NewName, entity.Name);
                Assert.Equal(dto.NewParentCategoryId, entity.ParentCategoryId);
            }
        );
    }

    [Fact]
    public void NameChangeOnly__Ok()
    {
        // arrange
        var company = FakeCompany();
        MockIdentity(company);

        var targetCategory = company.Categories.First();
        var dto = new V1UpdateCategory
        {
            CategoryId = targetCategory.Id,
            NewName = "aryabhata"
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var entity = _ctx.Categories.First(x => x.Id == dto.CategoryId);
                Assert.Equal(dto.NewName, entity.Name);
                Assert.Equal(targetCategory.ParentCategoryId, entity.ParentCategoryId);
            }
        );
    }

    [Fact]
    public void NotFoundById__NotFound()
    {
        // arrange
        var company = FakeCompany();
        MockIdentity(company);

        var dto = new V1UpdateCategory
        {
            CategoryId = Guid.NewGuid(),
            NewName = "aryabhata"
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void NotFoundByIdentityUser__NotFound()
    {
        // arrange
        var company = FakeCompany();
        MockIdentity();

        var dto = new V1UpdateCategory
        {
            CategoryId = company.Categories.First().Id,
            NewName = "aryabhata"
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    private void MockIdentity(Company? company = null)
    {
        var companyId = company == null ? Guid.NewGuid() : company.Id;

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(new AppUser { IsAdmin = true, CompanyId = companyId });
    }

    private Company FakeCompany()
    {
        return _forger.Fake<Company>(company =>
        {
            company.Employees = new List<AppUser>
            {
                _forger.Fake<AppUser>(au => au.Company = company)
            };

            company.Categories = Enumerable.Range(0, 3)
                .Select(_ => _forger.Fake<Category>(c => c.Company = company))
                .ToList();
        });
    }
}
