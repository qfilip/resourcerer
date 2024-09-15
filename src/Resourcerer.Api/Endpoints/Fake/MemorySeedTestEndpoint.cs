using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Logic.Fake;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Resourcerer.Api.Endpoints.Fake;

public class MemorySeedTestEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromQuery] int excerpts,
        [FromServices] Pipeline pipeline,
        [FromServices] MemorySeedTest.Handler handler)
    {
        return await pipeline.Pipe(handler, excerpts);
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Fake("memory"), HttpMethod.Get, Action, null);
}
