using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateCompositeItemProductionOrderEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1CreateCompositeItemProductionOrderCommand dto,
       [FromServices] CreateCompositeItemProductionOrder.Validator validator,
       [FromServices] IMessageSender<V1ItemProductionCommand> sender,
       [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            validator,
            sender,
            nameof(CreateCompositeItemProductionOrder));
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Instances("production_order/composite/create"), HttpMethod.Post, Action, MapAuth);
}
