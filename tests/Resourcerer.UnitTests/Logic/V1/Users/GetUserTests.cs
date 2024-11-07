using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Users;

public class GetUserTests : TestsBase
{
    private readonly GetUser.Handler _sut;
    public GetUserTests() => _sut = new(_ctx);

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var user = _forger.Fake<AppUser>();
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(user.Id).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.NotNull(result.Object),
            () => Assert.Null(result.Object!.Password)
        );
    }

    [Fact]
    public void UserNotFound__NotFound()
    {
        // arrange
        var user = _forger.Fake<AppUser>();
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(Guid.NewGuid()).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }
}
