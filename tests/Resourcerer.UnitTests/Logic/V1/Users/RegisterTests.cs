using MassTransit.JobService;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Api.Services.StaticServices;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Services;
using Resourcerer.Logic.Utilities.Query;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.Utilities.Cryptography;

namespace Resourcerer.UnitTests.Logic.V1.Users;

public class RegisterTests : TestsBase
{
    private readonly Register.Handler _sut;
    public RegisterTests() => _sut = new(_ctx, new());

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        AppStaticData.Auth.Jwt.Configure(Guid.Empty.ToString(), "issuer", "audience");
        var jwtService = new JwtTokenService(
            AppStaticData.Auth.Jwt.Key!,
            AppStaticData.Auth.Jwt.Issuer,
            AppStaticData.Auth.Jwt.Audience);
        var request = new V1Register
        {
            Username = "vaas",
            Password = "montenegro",
            Email = "vaas.montenegro@notmail.com",
            CompanyName = "island_trade_inc"
        };

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.NotNull(jwtService.GenerateToken(Mapping.Of(result.Object!)!, result.Object!.PermissionsMap!, result.Object!.DisplayName!, result.Object!.Company!.Name!)),
            () => Assert.True(string.IsNullOrEmpty(result.Object!.Password)),
            () =>
            {
                _ctx.Clear();

                var user = _ctx.AppUsers
                    .Where(x =>
                        x.Name == request.Username &&
                        x.PasswordHash == Hasher.GetSha256Hash(request.Password))
                    .Select(AppUsers.DefaultDtoProjection)
                    .First();

                var expected = new AppUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    IsAdmin = user.IsAdmin,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    PermissionsMap = user.PermissionsMap,
                    Company = new CompanyDto
                    {
                        Id = user.Company!.Id,
                        Name = user.Company!.Name
                    },
                };

                Assert.Equivalent(expected, result.Object);
            }
        );
    }

    [Fact]
    public void CompanyExists__Rejected()
    {
        // arrange
        var company = _forger.Fake<Company>(x => x.Name = "island_trade_inc");

        var request = new V1Register
        {
            Username = "vaas",
            Password = "montenegro",
            Email = "vaas.montenegro@notmail.com",
            CompanyName = company.Name
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void EmailExists__Exception()
    {
        // arrange
        var email = "vaas.montenegro@notmail.com";
        var company = _forger.Fake<AppUser>(x => x.Email = email);

        var request = new V1Register
        {
            Username = "vaas",
            Password = "montenegro",
            Email = email,
            CompanyName = company.Name
        };

        _ctx.SaveChanges();

        var action = async () =>
        {
            await _sut.Handle(request);
            return Task.CompletedTask;
        };

        // assert
        Assert.ThrowsAsync<DbUpdateException>(action);
    }
}
