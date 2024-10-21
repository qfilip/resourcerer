using FakeItEasy;
using Resourcerer.Application.Auth.Abstractions;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Companies;

public class ChangeCompanyNameTests : TestsBase
{
    private readonly ChangeCompanyName.Handler _sut;
    private readonly IAppIdentityService<AppUser> _fakeIdentityService = A.Fake<IAppIdentityService<AppUser>>();
    public ChangeCompanyNameTests()
    {
        _sut = new(_ctx, new(), _fakeIdentityService, GetMapster());
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var company = _forger.Fake<Company>(c =>
        {
            c.Employees = new List<AppUser>
            {
                _forger.Fake<AppUser>(u =>
                {
                    u.IsAdmin = true;
                    u.Company = c;
                })
            };
        });

        var dto = new V1ChangeCompanyName
        {
            CompanyId = company.Id,
            NewName = "eswatini"
        };

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(company.Employees.First());

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var entity = _ctx.Companies.First();
                Assert.Equal(dto.NewName, entity.Name);
            }
        );
    }

    [Fact]
    public void UserPermissions_NonAdmin__Rejected()
    {
        // arrange
        var company = _forger.Fake<Company>(c =>
        {
            c.Employees = new List<AppUser> { _forger.Fake<AppUser>(u => u.IsAdmin = false) };
        });

        var dto = new V1ChangeCompanyName
        {
            CompanyId = company.Id,
            NewName = "eswatini"
        };

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(company.Employees.First());

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void UserPermissions_DifferentCompany__Rejected()
    {
        // arrange
        var company = _forger.Fake<Company>();
        var user = _forger.Fake<AppUser>(u => u.IsAdmin = true);

        var dto = new V1ChangeCompanyName
        {
            CompanyId = company.Id,
            NewName = "eswatini"
        };

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(user);

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
