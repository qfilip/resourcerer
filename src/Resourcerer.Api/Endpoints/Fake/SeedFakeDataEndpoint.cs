using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Models;
using Resourcerer.Logic.Fake;

namespace Resourcerer.Api.Endpoints.Fake;

public class SeedFakeDataEndpoint
{
    public static async Task<IResult> Action(
        [FromServices] Pipeline pipeline,
        [FromServices] Seed.Handler handler)
    {
        return await pipeline.Pipe(handler, Unit.New);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("", Action);
    }
}
