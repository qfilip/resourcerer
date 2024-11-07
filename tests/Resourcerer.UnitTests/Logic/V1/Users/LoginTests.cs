using Resourcerer.Api.Services.StaticServices;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Identity.Services;
using Resourcerer.Identity.Utils;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.Utilities.Cryptography;
using SqlForgery;
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
        var jwtService = new JwtTokenService(
            AppStaticData.Auth.Jwt.Key!,
            AppStaticData.Auth.Jwt.Issuer,
            AppStaticData.Auth.Jwt.Audience);
        var (user, password) = ArrangeDb(_ctx, _forger);
        var request = new AppUserDto { Name = user.Name, Password = password };

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.NotNull(result.Object),
            () => Assert.NotNull(jwtService.GenerateToken(Mapping.Of(result.Object!)!, result.Object!.PermissionsMap!, result.Object!.DisplayName!, result.Object!.Company!.Name!)),
            () => Assert.True(string.IsNullOrEmpty(result.Object!.Password))
        );
    }

    [Fact]
    public void Username_NotFound__NotFound()
    {
        // arrange
        var (_, password) = ArrangeDb(_ctx, _forger);
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
        var (user, _) = ArrangeDb(_ctx, _forger);
        var request = new AppUserDto { Name = user.Name, Password = "gauguin" };

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    private static (AppUser, string) ArrangeDb(TestDbContext ctx, Forger forger)
    {
        var username = "village_person";
        var password = "y.m.c.a";

        var user = forger.Fake<AppUser>(x =>
        {
            x.Name = username;
            x.IsAdmin = true;
            x.DisplayName = "Villager";
            x.Email = "a@a.com";
            x.PasswordHash = Hasher.GetSha256Hash(password);
            x.Permissions = JsonSerializer.Serialize(Permissions.GetCompressed());
            x.Company = forger.Fake<Company>();
        });

        ctx.SaveChanges();

        return (user, password);
    }
}
