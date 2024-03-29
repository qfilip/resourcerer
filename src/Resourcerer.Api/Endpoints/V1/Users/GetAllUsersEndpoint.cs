﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Queries.V1;

namespace Resourcerer.Api.Endpoints.V1;

public static class GetAllUsersEndpoint
{
    public static async Task<IResult> Action(
        [FromServices] Pipeline pipeline,
        [FromServices] GetAllUsers.Handler handler)
    {
        return await pipeline.Pipe(handler, new Unit());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.User, new[] { ePermission.Read })
        });
    }
}
