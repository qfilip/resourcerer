using FakeItEasy;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1.Users;
using Resourcerer.Logic;
using Resourcerer.Logic.Utilities.Query;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Users;

public class RegisterUserTests : TestsBase
{
    private readonly RegisterUser.Handler _sut;
    private readonly IEmailService _fakeEmailService = A.Fake<IEmailService>();
    public RegisterUserTests()
    {
        _sut = new(_ctx, new(), _fakeEmailService);
    }

    [Fact]
    public void HappyPath__Ok()
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

        A.CallTo(() => _fakeEmailService.Validate(request.Email))
            .Returns(true);

        A.CallTo(() => _fakeEmailService.Send(A<string>.Ignored, request.Email))
            .Returns(Task.CompletedTask);

        // act
        var result = _sut.Handle(request).Await();

        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.True(string.IsNullOrEmpty(result.Object!.Password)),
            () =>
            {
                A.CallTo(() => _fakeEmailService.Validate(request.Email))
                    .MustHaveHappenedOnceExactly();

                A.CallTo(() => _fakeEmailService.Send(A<string>.Ignored, request.Email))
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
