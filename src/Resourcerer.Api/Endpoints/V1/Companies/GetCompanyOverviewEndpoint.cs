using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Logic.V1;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Resourcerer.Api.Endpoints.V1;

public class GetCompanyOverviewEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromQuery] Guid companyId,
        [FromServices] Pipeline pipeline,
        [FromServices] GetCompanyOverview.Handler handler)
    {
        return await pipeline.Pipe(handler, companyId);
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Companies("overview"), HttpMethod.Get, Action);
}
