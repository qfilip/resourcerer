using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1.Items;

namespace Resourcerer.Api.Endpoints.V1;

public class GetItemsStatisticsEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        Guid itemId,
        Pipeline pipeline,
        GetItemStatistics.Handler handler)
    {
        return await pipeline.Pipe(handler, (itemId, DateTime.UtcNow));
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
            EndpointMapper.Items("statistics"), eHttpMethod.Get, Action, MapAuth);
}
