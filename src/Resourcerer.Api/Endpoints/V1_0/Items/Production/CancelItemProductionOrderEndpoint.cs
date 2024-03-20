using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1_0.Commands.Items;

namespace Resourcerer.Api.Endpoints.V1_0;

public static class CancelItemProductionOrderEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] CancelItemProductionOrderRequestDto dto,
       [FromServices] ISenderAdapter<ItemProductionEventBaseDto> sender,
       [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            () => CancelItemProductionOrder.Handler.Validate(dto),
            sender,
            nameof(CancelItemProductionOrder));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/production_order/cancel", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
