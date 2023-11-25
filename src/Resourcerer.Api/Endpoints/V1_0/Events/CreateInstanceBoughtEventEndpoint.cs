using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.V1_0;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateInstanceBoughtEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] ItemOrderedEventDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateItemOrderedEvent.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, new ItemOrderedEventDto.Validator());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/bought", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
