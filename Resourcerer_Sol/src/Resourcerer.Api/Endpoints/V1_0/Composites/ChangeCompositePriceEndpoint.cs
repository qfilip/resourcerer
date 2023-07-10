﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Composites;

namespace Resourcerer.Api.Endpoints.V1_0.Composites;

public class ChangeCompositePriceEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] ChangePriceDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] ChangeCompositePrice.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/change-price", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Composite, new[] { ePermission.Write })
        });
    }
}
