using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Commands.Items;

namespace Resourcerer.Api.Endpoints.V1.Items.Production;

public class FinishItemProductionOrderEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1FinishItemProductionOrderRequest dto,
       [FromServices] ISenderAdapter<V1ItemProductionEvent> sender,
       [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            () => FinishItemProductionOrder.Validate(dto),
            sender,
            nameof(FinishItemProductionOrder));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/production_order/finish", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
