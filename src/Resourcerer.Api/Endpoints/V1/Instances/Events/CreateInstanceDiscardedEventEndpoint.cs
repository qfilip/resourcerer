using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1_0.Commands;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateInstanceDiscardedEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1InstanceDiscardedRequest dto,
        [FromServices] ISenderAdapter<V1InstanceDiscardedRequest> sender,
        [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            () => CreateInstanceDiscardedEvent.Handler.Validate(dto),
            sender,
            nameof(CreateInstanceDiscardedEvent));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order/discard", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
