using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Application.Models;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class GetAllCompaniesEndpoint
{
    public static async Task<IResult> Action(
        [FromServices] Pipeline pipeline,
        [FromServices] GetAllCompanies.Handler handler)
    {
        return await pipeline.Pipe(handler, Unit.New);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("", Action);
    }
}
