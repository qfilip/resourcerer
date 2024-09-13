using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateInstanceDiscardedEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1InstanceDiscardCommand dto,
        [FromServices] CreateInstanceDiscardedEvent.Validator validator,
        [FromServices] IMessageSender<V1InstanceDiscardCommand> sender,
        [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            validator,
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
