using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Endpoints.V1;

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
        EndpointMapper.AddAuthorization(endpoint, new List<(eResource claimType, ePermission[] claimValues)>
        {
            (eResource.ItemEvent, new[] { ePermission.Create })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Items("production/element/create"), eHttpMethod.Post, Action, MapAuth);
}
