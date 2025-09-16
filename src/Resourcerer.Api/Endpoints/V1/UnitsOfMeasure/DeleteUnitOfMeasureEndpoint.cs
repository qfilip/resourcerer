using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class DeleteUnitOfMeasureEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromQuery] Guid id,
       [FromServices] Pipeline pipeline,
       [FromServices] DeleteUnitOfMeasure.Handler handler)
    {
        return await pipeline.Pipe(handler, id);
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
            EndpointMapper.UnitsOfMeasure(""), eHttpMethod.Delete, Action, MapAuth);
}
