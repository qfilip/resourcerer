﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Queries.V1_0;

namespace Resourcerer.Api.Endpoints.V1_0;

public static class GetUserEndpoint
{
    public static async Task<IResult> Action(
        [FromQuery] Guid userId,
        [FromServices] Pipeline pipeline,
        [FromServices] GetUser.Handler handler)
    {
        return await pipeline.Pipe(handler, userId);
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