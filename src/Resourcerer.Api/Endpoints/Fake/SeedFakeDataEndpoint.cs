using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Models;
using Resourcerer.Logic.Fake;

namespace Resourcerer.Api.Endpoints.Fake;

public class SeedFakeDataEndpoint : IAppEndpoint
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

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Fake("seed"), eHttpMethod.Get, Action, null);
}
