using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class GetElementItemForEditEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromQuery] Guid itemId,
        [FromQuery] Guid companyId,
        Pipeline pipeline,
        GetElementItemForEdit.Handler handler)
    {
        return await pipeline.Pipe(handler, (itemId, companyId));
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Item, new[] { ePermission.Read })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Items("edit/element/form"), eHttpMethod.Get, Action, MapAuth);
}
