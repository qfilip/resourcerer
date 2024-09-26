using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class DeleteUnitOfMeasureEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       Guid id,
       [FromServices] Pipeline pipeline,
       [FromServices] DeleteUnitOfMeasure.Handler handler)
    {
        return await pipeline.Pipe(handler, id);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Item, new[] { ePermission.Remove })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.UnitsOfMeasure(""), eHttpMethod.Delete, Action, MapAuth);
}
