﻿using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;
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
            { eResource.User.ToString(), [ePermission.View.ToString()] }
        };

        var user = _forger.Fake<AppUser>(x => x.Permissions = JsonSerializer.Serialize(permissions));
        _ctx.SaveChanges();

        var request = new V1SetUserPermissions
        {
            UserId = user.Id,
            Permissions = new Dictionary<string, string[]>()
            {
                {
                    eResource.User.ToString(),
                    [ePermission.View.ToString(), ePermission.Create.ToString()]
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
        var user = _forger.Fake<AppUser>();
        _ctx.SaveChanges();

        var request = new V1SetUserPermissions
        {
            UserId = user.Id,
            Permissions = new Dictionary<string, string[]>()
            {
                {
                    eResource.User.ToString().ToLower(),
                    [ePermission.View.ToString(), ePermission.Create.ToString()]
                },
                {
                    eResource.User.ToString(),
                    [ePermission.View.ToString().ToLower(), ePermission.Create.ToString().ToLower()]
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
        var user = _forger.Fake<AppUser>();
        _ctx.SaveChanges();

        var request = new V1SetUserPermissions
        {
            UserId = Guid.NewGuid(),
            Permissions = new Dictionary<string, string[]>()
            {
                {
                    eResource.User.ToString(), [ePermission.View.ToString()]
                }
            }
        };

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }
}