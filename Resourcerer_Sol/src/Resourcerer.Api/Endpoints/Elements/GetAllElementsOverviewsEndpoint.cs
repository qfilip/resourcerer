using Resourcerer.Logic;
using Resourcerer.Logic.Queries.Elements;

namespace Resourcerer.Api.Endpoints.Elements;

public class GetAllElementsOverviewsEndpoint
{
    public static async Task<IResult> Action(
        Pipeline pipeline,
        GetAllElementsOverview.Handler handler)
    {
        return await pipeline.Pipe(handler, new Unit());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapGet("/all-overviews", Action);
    }
}
