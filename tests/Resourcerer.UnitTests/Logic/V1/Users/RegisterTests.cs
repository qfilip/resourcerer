using Resourcerer.Api;
using Resourcerer.Api.Services;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
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
            () => Assert.NotNull(JwtService.GenerateToken(result.Object!)),
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
    public void CompanyOrUserExists__Rejected()
    {
        // arrange
        var company = _forger.Fake<Company>(x => x.Name = "island_trade_inc");
        var user = _forger.Fake<AppUser>(x => x.Name = "vaas");
        var request = new V1Register
        {
            Username = user.Name,
            Password = "montenegro",
            Email = "vaas.montenegro@notmail.com",
            CompanyName = company.Name
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Rejected, result.Status),
            () => Assert.True(result.Errors.Length == 2)
        );
    }
}
