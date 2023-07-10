using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.ElementEvents;

namespace Resourcerer.Api.Endpoints.V1_0.ElementEvents;

public class CreateElementDeliveredEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] InstanceDeliveredEventDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateElementDeliveredEvent.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order-delivered", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Element, new[] { ePermission.Write })
        });
    }
}
