using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Users;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.Utilities.Cryptography;

namespace Resourcerer.UnitTests.Logic.V1.Users;

public class LoginTests : TestsBase
{
    private readonly Login.Handler _sut;
    public LoginTests() => _sut = new(_ctx, new());

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var username = "village_person";
        var password = "y.m.c.a";
        
        DF.Fake<AppUser>(_ctx, x =>
        {
            x.Name = username;
            x.PasswordHash = Hasher.GetSha256Hash(password);
        });

        var request = new AppUserDto { Name = username, Password = password };
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.NotNull(result.Object),
            () => Assert.NotNull(result.Object!.Name)
        );
    }
}
