using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.ElementEvents;

namespace Resourcerer.Api.Endpoints.V1_0.ElementEvents;

public class CreateElementOrderedEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] InstanceOrderedEventDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateElementOrderedEvent.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/ordered", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Element, new[] { ePermission.Write })
        });
    }
}
