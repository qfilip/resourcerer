using Resourcerer.Logic;
using Resourcerer.Logic.Elements.Queries;

namespace Resourcerer.Api.Endpoints.Elements;

public class GetAllElementsOverviewsEndpoint
{
    public static async Task<IResult> Action(
        Pipeline pipeline,
        GetAllElementsOverview.Handler handler)
    {
        return await pipeline.Pipe(handler, new Unit(), nameof(GetAllElementsOverview));
    }
}
