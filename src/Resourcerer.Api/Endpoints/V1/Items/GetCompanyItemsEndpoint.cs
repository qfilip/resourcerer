using Resourcerer.Api.Services;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class GetCompanyItemsEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        Guid companyId,
        Pipeline pipeline,
        GetCompanyItems.Handler handler)
    {
        return await pipeline.Pipe(handler, companyId);
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
            EndpointMapper.Items(""), eHttpMethod.Get, Action, MapAuth);
}
