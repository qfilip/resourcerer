using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Resourcerer.Api.Endpoints.V1.Items.Production;

public class FinishItemProductionOrderEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1FinishItemProductionOrderCommand dto,
       [FromServices] FinishItemProductionOrder.Validator validator,
       [FromServices] IMessageSender<V1ItemProductionCommand> sender,
       [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            validator,
            sender,
            nameof(FinishItemProductionOrder));
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Instances("production_order/finish"), HttpMethod.Post, Action, MapAuth);
}
