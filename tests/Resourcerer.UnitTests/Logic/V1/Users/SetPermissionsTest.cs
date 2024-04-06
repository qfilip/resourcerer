using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Users;
using Resourcerer.UnitTests.Utilities;
using System.Text.Json;

namespace Resourcerer.UnitTests.Logic.V1.Users;

public class SetPermissionsTest : TestsBase
{
    private readonly SetPermissions.Handler _sut;
    public SetPermissionsTest() => _sut = new(_ctx, new());

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var permissions = new Dictionary<string, string[]>()
        {
            { ePermissionSection.User.ToString(), [ePermission.Read.ToString()] }
        };

        var user = DF.Fake<AppUser>(_ctx, x => x.Permissions = JsonSerializer.Serialize(permissions));
        _ctx.SaveChanges();

        var request = new V1SetUserPermissions
        {
            UserId = user.Id,
            Permissions = new Dictionary<string, string[]>()
            {
                { 
                    ePermissionSection.User.ToString(),
                    [ePermission.Read.ToString(), ePermission.Write.ToString()]
                }
            }
        };

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                var expected = new AppUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    PermissionsMap = request.Permissions
                };

                Assert.Equivalent(expected, result.Object);
            }
        );
    }

    [Fact]
    public void InvalidPermissionValues__Rejected()
    {
        // arrange
        var user = DF.Fake<AppUser>(_ctx);
        _ctx.SaveChanges();

        var request = new V1SetUserPermissions
        {
            UserId = user.Id,
            Permissions = new Dictionary<string, string[]>()
            {
                {
                    ePermissionSection.User.ToString().ToLower(),
                    [ePermission.Read.ToString(), ePermission.Write.ToString()]
                },
                {
                    ePermissionSection.User.ToString(),
                    [ePermission.Read.ToString().ToLower(), ePermission.Write.ToString().ToLower()]
                }
            }
        };

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Rejected, result.Status),
            () => Assert.True(result.Errors.Length == 3)
        );
    }

    [Fact]
    public void UserNotFound__NotFound()
    {
        // arrange
        var user = DF.Fake<AppUser>(_ctx);
        _ctx.SaveChanges();

        var request = new V1SetUserPermissions
        {
            UserId = Guid.NewGuid(),
            Permissions = new Dictionary<string, string[]>()
            {
                {
                    ePermissionSection.User.ToString(), [ePermission.Read.ToString()]
                }
            }
        };

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }
}