using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Users;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.UnitTests.Logic.V1.Users;

public class RegisterTests : TestsBase
{
    private readonly Register.Handler _sut;
    public RegisterTests() => _sut = new(_ctx, new());

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var request = new V1Register
        {
            Username = "vaas",
            Password = "montenegro",
            CompanyName = "island_trade_inc"
        };

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var user = _ctx.AppUsers
                    .Include(x => x.Company)
                    .First(x =>
                        x.Name == request.Username &&
                        x.PasswordHash == Hasher.GetSha256Hash(request.Password));
                
                var expected = new AppUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    IsAdmin = user.IsAdmin,
                    Company = new CompanyDto
                    {
                        Id = user.Company!.Id,
                        Name = user.Company!.Name
                    },
                    PermissionsMap = Permissions.GetPermissionsMap(user.Permissions!)
                };

                Assert.Equivalent(expected, result.Object);
            }
        );
    }

    [Fact]
    public void CompanyOrUserExists__Rejected()
    {
        // arrange
        var company = DF.Fake<Company>(_ctx, x => x.Name = "island_trade_inc");
        var user = DF.Fake<AppUser>(_ctx, x => x.Name = "vaas");
        var request = new V1Register
        {
            Username = user.Name,
            Password = "montenegro",
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
