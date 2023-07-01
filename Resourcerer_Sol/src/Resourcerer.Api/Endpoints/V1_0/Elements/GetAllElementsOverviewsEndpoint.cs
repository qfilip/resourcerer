using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Queries.Elements;

namespace Resourcerer.Api.Endpoints.V1_0.Elements;

public class GetAllElementsOverviewsEndpoint
{
    public static async Task<IResult> Action(
        Pipeline pipeline,
        GetAllElementsStatistics.Handler handler)
    {
        return await pipeline.Pipe(handler, new Unit());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("/all-overviews", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(string claimType, string[] claimValues)>
        {
            (nameof(Element), new[] { ePermission.Read.ToString() })
        });
    }
}
