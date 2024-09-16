using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Models;
using Resourcerer.Logic.V1;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Resourcerer.Api.Endpoints.V1;

public class GetAllCompaniesEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromServices] Pipeline pipeline,
        [FromServices] GetAllCompanies.Handler handler)
    {
        return await pipeline.Pipe(handler, Unit.New);
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Companies(""), HttpMethod.Get, Action);
}
