using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.V1_0;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateItemDeliveredEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] ItemDeliveredEventDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateItemDeliveredEvent.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, new ItemDeliveredEventDto.Validator());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order-deliver", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
