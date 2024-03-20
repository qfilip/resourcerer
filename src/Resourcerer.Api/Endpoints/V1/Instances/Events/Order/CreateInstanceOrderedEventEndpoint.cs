using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Commands;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateInstanceOrderedEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1InstanceOrderRequest dto,
        [FromServices] ISenderAdapter<V1InstanceOrderEvent> sender,
        [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            () => CreateInstanceOrderedEvent.Handler.Validate(dto),
            sender,
            nameof(CreateInstanceOrderedEvent));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
