using Resourcerer.Api;
using Resourcerer.Api.Services;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.UnitTests.Logic.V1.Users;

public class LoginTests : TestsBase
{
    private readonly Login.Handler _sut;
    public LoginTests() => _sut = new(_ctx, new());

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        AppStaticData.Auth.Jwt.Configure(Guid.Empty.ToString(), "issuer", "audience");
        var (user, password) = ArrangeDb(_ctx);
        var request = new AppUserDto { Name = user.Name, Password = password };

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.NotNull(result.Object),
            () => Assert.NotNull(JwtService.GenerateToken(result.Object!)),
            () => Assert.True(string.IsNullOrEmpty(result.Object!.Password))
        );
    }

    [Fact]
    public void Username_NotFound__NotFound()
    {
        // arrange
        var (_, password) = ArrangeDb(_ctx);
        var request = new AppUserDto { Name = "fnaah", Password = password };

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void BadPassword__Rejected()
    {
        // arrange
        var (user, _) = ArrangeDb(_ctx);
        var request = new AppUserDto { Name = user.Name, Password = "gauguin" };

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    private static (AppUser, string) ArrangeDb(TestDbContext ctx)
    {
        var username = "village_person";
        var password = "y.m.c.a";

        var user = DF.Fake<AppUser>(ctx, x =>
        {
            x.Name = username;
            x.IsAdmin = true;
            x.DisplayName = "Villager";
            x.Email = "a@a.com";
            x.PasswordHash = Hasher.GetSha256Hash(password);
            x.Permissions = JsonSerializer.Serialize(Permissions.GetCompressed());
            x.Company = DF.Fake<Company>(ctx);
        });

        ctx.SaveChanges();

        return (user, password);
    }
}
