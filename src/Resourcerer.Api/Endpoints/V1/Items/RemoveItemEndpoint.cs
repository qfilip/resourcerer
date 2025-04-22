using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class RemoveItemEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
      [FromQuery] Guid itemId,
      [FromServices] Pipeline pipeline,
      [FromServices] RemoveItem.Handler handler)
    {
        return await pipeline.Pipe(handler, itemId);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eResource claimType, ePermission[] claimValues)>
        {
            (eResource.Item, new[] { ePermission.Remove })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Items(""), eHttpMethod.Delete, Action, MapAuth);
}
