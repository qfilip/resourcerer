using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Queries.Items;

namespace Resourcerer.Api.Endpoints.V1_0.Items;

public class GetItemsStatisticsEndpoint
{
    public static async Task<IResult> Action(
        Guid itemId,
        Pipeline pipeline,
        GetItemStatistics.Handler handler)
    {
        return await pipeline.PipeGet(handler, (itemId, DateTime.UtcNow));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("/statistics", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Item, new[] { ePermission.Read })
        });
    }
}
