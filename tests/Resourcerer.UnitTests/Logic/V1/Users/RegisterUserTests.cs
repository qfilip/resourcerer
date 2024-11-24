using FakeItEasy;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Enums;
using Resourcerer.Identity.Models;
using Resourcerer.Identity.Utils;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.Utilities.Query;
using Resourcerer.Logic.V1;
using Resourcerer.Messaging.Emails.Abstractions;
using Resourcerer.Messaging.Emails.Models;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Users;

public class RegisterUserTests : TestsBase
{
    private readonly RegisterUser.Handler _sut;
    private readonly IEmailSender _fakeEmailService = A.Fake<IEmailSender>();
    private readonly IAppIdentityService<AppIdentity> _fakeIdentityService = A.Fake<IAppIdentityService<AppIdentity>>();
    public RegisterUserTests()
    {
        _sut = new(_ctx, new(), _fakeEmailService, _fakeIdentityService);
    }

    [Fact]
    public void HappyPath_AdminAddsAdmin__Ok()
    {
        // arrange
        var company = _forger.Fake<Company>();
        
        var request = GetRequest(company);

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(true, Guid.NewGuid()));

        HappyPathActAssert(request);
    }

    [Fact]
    public void HappyPath_AdminAddsNonAdmin__Ok()
    {
        // arrange
        var company = _forger.Fake<Company>();

        var request = GetRequest(company, x => x.IsAdmin = false);

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(true, Guid.NewGuid()));

        HappyPathActAssert(request);
    }

    [Fact]
    public void HappyPath_NonAdminAddsNonAdmin__Ok()
    {
        // arrange
        var company = _forger.Fake<Company>();

        var request = GetRequest(company, x => x.IsAdmin = false);

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(false, Guid.NewGuid()));

        HappyPathActAssert(request);
    }

    [Fact]
    public void HappyPath_NonAdminAddsAdmin__Rejected()
    {
        // arrange
        var company = _forger.Fake<Company>();

        var request = GetRequest(company);

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(false, Guid.NewGuid()));

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void InvalidPermissions__Rejected()
    {
        // arrange
        var company = _forger.Fake<Company>();

        var request = GetRequest(company, x =>
        {
            x.PermissionsMap = new Dictionary<string, string[]>
            {
                {
                    eSection.User.ToString(), ["one", "two"]
                }
            };
        });

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
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Rejected, result.Status),
            () => Assert.Equal(2, result.Errors.Length)
        );
    }

    [Fact]
    public void InvalidEmail__Rejected()
    {
        // arrange
        var company = _forger.Fake<Company>();

        var request = GetRequest(company);

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(true, Guid.NewGuid()));

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
        var company = _forger.Fake<Company>();

        var request = GetRequest(company);

        _ctx.SaveChanges();

        A.CallTo(() =>
            _fakeIdentityService.Get())
            .Returns(DataFaking.Identity(true, Guid.NewGuid()));

        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);

        A.CallTo(() => _fakeEmailService.SendAsync(A<Email>.That.Matches(x => x.Address == request.Email)))
            .Throws(new Exception());

        // act
        var action = () => _sut.Handle(request).Await();

        // assert
        Assert.Throws<Exception>(action);
    }

    private static V1RegisterUser GetRequest(Company company, Action<V1RegisterUser>? modifier = null)
    {
        var request = new V1RegisterUser
        {
            CompanyId = company.Id,
            Username = DF.MakeName(),
            Email = DF.MakeEmail(),
            IsAdmin = true,
            PermissionsMap = Permissions.GetPermissionsMap(Permissions.GetCompressed())
        };

        modifier?.Invoke(request);

        return request;
    }

    private void HappyPathActAssert(V1RegisterUser request)
    {
        A.CallTo(() =>
            _fakeEmailService.Validate(A<string>.That.Matches(x => x == request.Email)))
            .Returns(true);

        A.CallTo(() => _fakeEmailService.SendAsync(A<Email>.That.Matches(x => x.Address == request.Email)))
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

                A.CallTo(() => _fakeEmailService.SendAsync(A<Email>.Ignored))
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
