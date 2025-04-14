using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1.Instances.Events.Order;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateInstanceOrderCancelledEventEndpoint : IAppEndpoint
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

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eResource claimType, ePermission[] claimValues)>
        {
            (eResource.InstanceEvent, new[] { ePermission.Create })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Instances("order/cancel"), eHttpMethod.Post, Action, MapAuth);
}
