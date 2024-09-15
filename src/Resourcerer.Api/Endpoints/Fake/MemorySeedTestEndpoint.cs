using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Logic.Fake;

namespace Resourcerer.Api.Endpoints.Fake;

public class MemorySeedTestEndpoint
{
    public static async Task<IResult> Action(
        [FromQuery] int excerpts,
        [FromServices] Pipeline pipeline,
        [FromServices] MemorySeedTest.Handler handler)
    {
        return await pipeline.Pipe(handler, excerpts);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("memory_seed", Action);
    }

    //public AppEndpoint GetEndpointInfo() => new AppEndpoint(1, 0, EndpointMapper.SeedGroup)
}
