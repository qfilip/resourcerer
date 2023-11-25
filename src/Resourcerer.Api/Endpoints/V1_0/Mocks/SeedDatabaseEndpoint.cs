using Resourcerer.Api.Services;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Mocks;

namespace Resourcerer.Api.Endpoints.V1_0.Mocks;

public class SeedDatabaseEndpoint
{
    public static async Task<IResult> Action(
        Pipeline pipeline,
        SeedMockData.Handler seedHandler)
    {
        return await pipeline.Pipe(seedHandler, new Unit());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapPost("/seed", Action);
    }
}
