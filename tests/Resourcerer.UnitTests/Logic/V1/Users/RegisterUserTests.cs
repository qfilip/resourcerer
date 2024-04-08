using FakeItEasy;
using Resourcerer.Application.Abstractions.Services;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Utilities.Query;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Users;

public class RegisterUserTests : TestsBase
{
    private readonly RegisterUser.Handler _sut;
    private readonly IEmailService _fakeEmailService = A.Fake<IEmailService>();
    private readonly IAppIdentityService<AppUser> _fakeIdentityService = A.Fake<IAppIdentityService<AppUser>>();
    public RegisterUserTests()
    {
        _sut = new(_ctx, new(), _fakeEmailService, _fakeIdentityService);
    }

    [Fact]
    public void HappyPath_AdminAddsAdmin__Ok()
    {
        // arrange
        var company = DF.Fake<Company>(_ctx);
        
        var request = new V1RegisterUser
        {
            CompanyId = company.Id,
            Username = DF.MakeName(),
            Email = DF.MakeEmail(),
            IsAdmin = true,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(new AppUser { IsAdmin = true });

        HappyPathActAssert(request);
    }

    [Fact]
    public void HappyPath_AdminAddsNonAdmin__Ok()
    {
        // arrange
        var company = DF.Fake<Company>(_ctx);

        var request = new V1RegisterUser
        {
            CompanyId = company.Id,
            Username = DF.MakeName(),
            Email = DF.MakeEmail(),
            IsAdmin = false,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(new AppUser { IsAdmin = true });

        HappyPathActAssert(request);
    }

    [Fact]
    public void HappyPath_NonAdminAddsNonAdmin__Ok()
    {
        // arrange
        var company = DF.Fake<Company>(_ctx);

        var request = new V1RegisterUser
        {
            CompanyId = company.Id,
            Username = DF.MakeName(),
            Email = DF.MakeEmail(),
            IsAdmin = false,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(new AppUser { IsAdmin = false });

        HappyPathActAssert(request);
    }

    [Fact]
    public void HappyPath_NonAdminAddsAdmin__Ok()
    {
        // arrange
        var company = DF.Fake<Company>(_ctx);

        var request = new V1RegisterUser
        {
            CompanyId = company.Id,
            Username = DF.MakeName(),
            Email = DF.MakeEmail(),
            IsAdmin = true,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(new AppUser { IsAdmin = false });

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void InvalidPermissions__Rejected()
    {
        // arrange
        var company = DF.Fake<Company>(_ctx);

        var request = new V1RegisterUser
        {
            CompanyId = company.Id,
            Username = DF.MakeName(),
            Email = DF.MakeEmail(),
            IsAdmin = true,
            PermissionsMap = new Dictionary<string, string[]>
            {
                { 
                    ePermissionSection.User.ToString(), ["one", "two"]
                }
            }
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(new AppUser { IsAdmin = true });

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Rejected, result.Status),
            () => Assert.Equal(2, result.Errors.Length)
        );
    }

    [Fact]
    public void InvalidEmail__Rejected()
    {
        // arrange
        var company = DF.Fake<Company>(_ctx);

        var request = new V1RegisterUser
        {
            CompanyId = company.Id,
            Username = DF.MakeName(),
            Email = DF.MakeEmail(),
            IsAdmin = true,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(new AppUser { IsAdmin = true });

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(false);

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Rejected, result.Status),
            () => Assert.Single(result.Errors)
        );
    }

    [Fact]
    public void FailedToSendEmail__Exception()
    {
        // arrange
        var company = DF.Fake<Company>(_ctx);

        var request = new V1RegisterUser
        {
            CompanyId = company.Id,
            Username = DF.MakeName(),
            Email = DF.MakeEmail(),
            IsAdmin = true,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(new AppUser { IsAdmin = true });

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);

        A.CallTo(() => _fakeEmailService.Send(A<string>.Ignored, A<string>.That.Matches(x => x == request.Email)))
            .Throws(new Exception());

        // act
        var action = () => _sut.Handle(request).Await();

        // assert
        Assert.Throws<Exception>(action);
    }

    private void HappyPathActAssert(V1RegisterUser request)
    {
        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);

        A.CallTo(() => _fakeEmailService.Send(A<string>.Ignored, A<string>.That.Matches(x => x == request.Email)))
            .Returns(Task.CompletedTask);


        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.True(string.IsNullOrEmpty(result.Object!.Password)),
            () =>
            {
                A.CallTo(() => _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
                    .MustHaveHappenedOnceExactly();

                A.CallTo(() => _fakeEmailService.Send(A<string>.Ignored, request.Email!))
                    .MustHaveHappenedOnceExactly();

                _ctx.Clear();

                var user = _ctx.AppUsers
                    .Select(AppUsers.DefaultDtoProjection)
                    .First();

                Assert.Equal(request.CompanyId, user.Company!.Id);
                Assert.Equal(request.Username, user.Name);
                Assert.Equal(request.Email, user.Email);
                Assert.Equal(request.IsAdmin, user.IsAdmin);
                Assert.Equal(request.PermissionsMap, user.PermissionsMap);
            }
        );
    }
}
