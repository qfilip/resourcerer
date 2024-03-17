﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1_0.Commands.Items;
using System.Threading.Channels;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateItemProductionOrderEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] CreateItemProductionOrderRequestDto dto,
       [FromServices] ChannelWriter<ItemProductionEventBaseDto> writer,
       [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeToChannel(
            dto,
            () => CreateItemProductionOrder.Handler.ValidateRequest(dto),
            writer,
            nameof(CreateItemProductionOrder));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/production-order", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
