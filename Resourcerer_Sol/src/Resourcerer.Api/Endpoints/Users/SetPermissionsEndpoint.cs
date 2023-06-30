﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Users;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Users;

namespace Resourcerer.Api.Endpoints.Users;

public class SetPermissionsEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] SetUserPermissionsDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] SetPermissions.Handler handler)
    {
        return await
            pipeline
            .Pipe<SetUserPermissionsDto, SetUserPermissionsDtoValidator, Unit>(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapPost("/set-permissions", Action)
            .RequireAuthorization(cfg =>
            {
                cfg.RequireClaim(nameof(AppUser), ePermission.Write.ToString());
            });
    }
}
