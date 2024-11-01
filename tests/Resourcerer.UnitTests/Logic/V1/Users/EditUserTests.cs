using FakeItEasy;
using Resourcerer.Application.Messaging.Emails.Abstractions;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;
using Resourcerer.Identity.Utils;
using Resourcerer.Logic.Utilities.Query;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Users;

public class EditUserTests : TestsBase
{
    private readonly EditUser.Handler _sut;
    private readonly IEmailSender _fakeEmailService = A.Fake<IEmailSender>();
    private readonly IAppIdentityService<AppIdentity> _fakeIdentityService = A.Fake<IAppIdentityService<AppIdentity>>();
    
    public EditUserTests()
    {
        _sut = new(_ctx, new(), _fakeEmailService, _fakeIdentityService);
    }

    [Fact]
    public void AdminEditsAdmin__Ok()
    {
        // arrange
        var oldUser = _forger.Fake<AppUser>(x => x.IsAdmin = true);

        var request = new V1EditUser
        {
            UserId = oldUser.Id,
            Email = DataFaking.MakeEmail(),
            IsAdmin = false,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(true,oldUser.Company!.Id));

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);


        // act assert
        HappyPathActAssert(request);
    }

    [Fact]
    public void AdminEditsNonAdmin__Ok()
    {
        // arrange
        var oldUser = _forger.Fake<AppUser>();

        var request = new V1EditUser
        {
            UserId = oldUser.Id,
            Email = DataFaking.MakeEmail(),
            IsAdmin = true,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(true, oldUser.Company!.Id));

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);


        // act assert
        HappyPathActAssert(request);
    }

    [Fact]
    public void NonAdminEditsNonAdmin__Ok()
    {
        // arrange
        var oldUser = _forger.Fake<AppUser>();

        var request = new V1EditUser
        {
            UserId = oldUser.Id,
            Email = DataFaking.MakeEmail(),
            IsAdmin = false,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(false, oldUser.Company!.Id));

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);

        // act assert
        HappyPathActAssert(request);
    }

    [Fact]
    public void NonAdminEditsAdmin__Rejected()
    {
        // arrange
        var oldUser = _forger.Fake<AppUser>(x => x.IsAdmin = true);

        var request = new V1EditUser
        {
            UserId = oldUser.Id,
            Email = DataFaking.MakeEmail(),
            IsAdmin = false,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(false, oldUser.Company!.Id));

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);

        // act
        var result = _sut.Handle(request).Await();
        
        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void InvalidPermissions__Rejected()
    {
        // arrange
        var oldUser = _forger.Fake<AppUser>();

        var request = new V1EditUser
        {
            UserId = oldUser.Id,
            Email = DataFaking.MakeEmail(),
            IsAdmin = false,
            PermissionsMap = new Dictionary<string, string[]> { { "one", ["two"] } }
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(true, oldUser.Company!.Id));

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void InvalidEmail__Rejected()
    {
        // arrange
        var oldUser = _forger.Fake<AppUser>();

        var request = new V1EditUser
        {
            UserId = oldUser.Id,
            Email = DataFaking.MakeEmail(),
            IsAdmin = false,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(true, oldUser.Company!.Id));

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(false);

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void UserNotFound__NotFound()
    {
        // arrange
        var oldUser = _forger.Fake<AppUser>();

        var request = new V1EditUser
        {
            UserId = Guid.NewGuid(),
            Email = DataFaking.MakeEmail(),
            IsAdmin = true,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(true, oldUser.Company!.Id));

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void EditorFromDifferentCompany__Rejected()
    {
        // arrange
        var oldUser = _forger.Fake<AppUser>();

        var request = new V1EditUser
        {
            UserId = oldUser.Id,
            Email = DataFaking.MakeEmail(),
            IsAdmin = true,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(true, Guid.NewGuid()));

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    private void HappyPathActAssert(V1EditUser request)
    {
        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);

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

                _ctx.Clear();

                var user = _ctx.AppUsers
                    .Select(AppUsers.DefaultDtoProjection)
                    .First();

                Assert.Equal(request.Email, user.Email);
                Assert.Equal(request.IsAdmin, user.IsAdmin);
                Assert.Equal(request.PermissionsMap, user.PermissionsMap);
            }
        );
    }
}
