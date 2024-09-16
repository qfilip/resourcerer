using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

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
        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Instances("order/discard"), HttpMethod.Post, Action, MapAuth);
}
