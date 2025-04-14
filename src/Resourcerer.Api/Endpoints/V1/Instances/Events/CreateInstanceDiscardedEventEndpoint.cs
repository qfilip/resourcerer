using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1.Instances.Events;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateInstanceDiscardedEventEndpoint : IAppEndpoint
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

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eResource claimType, ePermission[] claimValues)>
        {
            (eResource.InstanceEvent, new[] { ePermission.Create })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Instances("order/discard"), eHttpMethod.Post, Action, MapAuth);
}
