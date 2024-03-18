using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances.Events;
using Resourcerer.Logic.V1_0.Commands;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateInstanceDiscardedEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] InstanceDiscardedRequestDto dto,
        [FromServices] ISenderAdapter<InstanceDiscardedRequestDto> sender,
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
        var endpoint = group.MapPost("/discard", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
