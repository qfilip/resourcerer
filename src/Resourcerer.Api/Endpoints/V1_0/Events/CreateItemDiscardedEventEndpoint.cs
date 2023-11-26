using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1_0.Commands;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateItemDiscardedEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] ItemDiscardedEventDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateItemDiscardedEvent.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/discard", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
