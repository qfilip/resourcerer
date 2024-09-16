using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos.V1;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1.Items.Events.Production;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Resourcerer.Api.Endpoints.V1.Items.Production;

public class CreateElementItemProductionOrderEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1CreateElementItemProductionOrderCommand dto,
       [FromServices] CreateElementItemProductionOrder.Validator validator,
       [FromServices] IMessageSender<V1ItemProductionCommand> sender,
       [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            validator,
            sender,
            nameof(CreateElementItemProductionOrder));
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Items("production_order/element/create"), HttpMethod.Post, Action, MapAuth);
}
