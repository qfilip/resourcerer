using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class GetItemTypeEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromQuery] Guid itemId,
       [FromServices] Pipeline pipeline,
       [FromServices] GetItemType.Handler handler)
    {
        return await pipeline.Pipe(handler, itemId);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eResource claimType, ePermission[] claimValues)>
        {
            (eResource.Item, new[] { ePermission.View })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Items("type"), eHttpMethod.Get, Action, MapAuth);
}
