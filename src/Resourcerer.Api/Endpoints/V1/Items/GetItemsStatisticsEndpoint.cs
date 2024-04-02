using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1.Items;

namespace Resourcerer.Api.Endpoints.V1;

public class GetItemsStatisticsEndpoint
{
    public static async Task<IResult> Action(
        Guid itemId,
        Pipeline pipeline,
        GetItemStatistics.Handler handler)
    {
        return await pipeline.Pipe(handler, (itemId, DateTime.UtcNow));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("/statistics", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Read })
        });
    }
}
