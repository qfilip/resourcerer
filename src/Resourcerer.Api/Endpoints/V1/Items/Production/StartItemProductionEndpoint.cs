using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Commands.Items;

namespace Resourcerer.Api.Endpoints.V1;

public class StartItemProductionEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1StartItemProductionOrderRequest dto,
       [FromServices] ISenderAdapter<V1ItemProductionEvent> sender,
       [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            () => StartItemProductionOrder.Validate(dto),
            sender,
            nameof(StartItemProductionOrder));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/production_order/start", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
