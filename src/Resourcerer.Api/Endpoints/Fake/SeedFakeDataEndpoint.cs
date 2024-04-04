using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.Fake;
using Resourcerer.Logic.Fake;

namespace Resourcerer.Api.Endpoints.Fake;

public class SeedFakeDataEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] DataSeedDto seedDto,
        [FromServices] Pipeline pipeline,
        [FromServices] Seed.Handler handler)
    {
        return await pipeline.Pipe(handler, seedDto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("", Action);
    }
}
