using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateInstanceOrderedEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1InstanceOrderCreateCommand dto,
        [FromServices] V1CreateInstanceOrderedEvent.Validator validator,
        [FromServices] IMessageSender<V1InstanceOrderCommand> sender,
        [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            validator,
            sender,
            nameof(V1CreateInstanceOrderedEvent));
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
