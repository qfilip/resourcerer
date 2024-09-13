using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateInstanceOrderCancelledEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1InstanceOrderCancelCommand dto,
        [FromServices] CreateInstanceOrderCancelledEvent.Validator validator,
        [FromServices] IMessageSender<V1InstanceOrderCommand> sender,
        [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            validator,
            sender,
            nameof(CreateInstanceOrderCancelledEvent));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order/cancel", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
