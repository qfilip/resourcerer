using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos.V1;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1.Items.Events.Production;

namespace Resourcerer.Api.Endpoints.V1.Items.Production;

public class CancelElementItemProductionOrderEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1CancelElementItemProductionOrderCommand dto,
       [FromServices] CancelElementItemProductionOrder.Validator validator,
       [FromServices] IMessageSender<V1ItemProductionCommand> sender,
       [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            validator,
            sender,
            nameof(CancelElementItemProductionOrder));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/production_order/element/cancel", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
