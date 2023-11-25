using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.Events;

namespace Resourcerer.Api.Endpoints.V1_0.Events;

public class CreateInstanceOrderedEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] InstanceOrderedEventDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateInstanceOrderedEvent.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, new InstanceOrderedEventDto.Validator());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/ordered", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
