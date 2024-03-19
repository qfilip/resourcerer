using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1_0.Commands.Items;

namespace Resourcerer.Api.Endpoints.V1_0.Items.Production;

public class FinishItemProductionOrderEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] FinishItemProductionOrderRequest dto,
       [FromServices] ISenderAdapter<ItemProductionEventBaseDto> sender,
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
        var endpoint = group.MapPost("/production-order-finish", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
