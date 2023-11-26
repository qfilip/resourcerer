using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.V1_0;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateItemOrderCancelledEventEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] ItemOrderCancelledEventDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateItemOrderCancelledEvent.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, new ItemOrderCancelledEventDto.Validator());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order-cancel", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
