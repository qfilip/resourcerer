﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.Items;

namespace Resourcerer.Api.Endpoints.V1_0.Items;

public class ChangeItemPriceEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] ChangePriceDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] ChangeItemPrice.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/change-price", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Item, new[] { ePermission.Write })
        });
    }
}