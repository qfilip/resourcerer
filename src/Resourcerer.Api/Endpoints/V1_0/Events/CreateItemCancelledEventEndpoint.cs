using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.V1_0;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateItemCancelledEventEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] ItemCancelledEventDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateItemCancelledEvent.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/cancel", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
