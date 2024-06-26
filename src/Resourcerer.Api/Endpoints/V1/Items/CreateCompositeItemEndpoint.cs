﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateCompositeItemEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1CreateCompositeItem dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateCompositeItem.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create/composite", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Item, new[] { ePermission.Write })
        });
    }
}
