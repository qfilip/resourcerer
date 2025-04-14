using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateCategoryEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1CreateCategory categoryDto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateCategory.Handler handler)
    {
        return await pipeline.Pipe(handler, categoryDto);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eResource claimType, ePermission[] claimValues)>
        {
            (eResource.Category, new[] { ePermission.Create })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Categories(""), eHttpMethod.Post, Action, MapAuth);
}
