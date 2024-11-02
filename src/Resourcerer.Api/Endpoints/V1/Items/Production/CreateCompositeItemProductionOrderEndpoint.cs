﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateCompositeItemProductionOrderEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1CreateCompositeItemProductionOrderCommand dto,
       [FromServices] CreateCompositeItemProductionOrder.Validator validator,
       [FromServices] IMessageSender<V1ItemProductionCommand> sender,
       [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            validator,
            sender,
            nameof(CreateCompositeItemProductionOrder));
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Item, new[] { ePermission.Create })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Items("production_order/composite/create"), eHttpMethod.Post, Action, MapAuth);
}
