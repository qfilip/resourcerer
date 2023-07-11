using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.Events;

namespace Resourcerer.Api.Endpoints.V1_0.Events;

public class CreateInstanceOrderCancelledEventEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] InstanceOrderCancelledEventDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateInstanceOrderCancelledEvent.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order-cancelled", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Item, new[] { ePermission.Write })
        });
    }
}
