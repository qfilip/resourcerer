using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Identity.Enums;
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
        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Item, new[] { ePermission.View })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Items("edit/element/form"), eHttpMethod.Get, Action, MapAuth);
}
