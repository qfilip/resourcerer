using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.Events;

namespace Resourcerer.Api.Endpoints.V1_0.Events;

public class CreateInstanceDeliveredEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] InstanceDeliveredEventDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateInstanceDeliveredEvent.Handler handler)
    {
        return await pipeline.PipeWithValidator(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order-delivered", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
