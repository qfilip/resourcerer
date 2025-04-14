using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.Entity;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class RemoveCategoryEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] CategoryDto categoryDto,
        [FromServices] Pipeline pipeline,
        [FromServices] RemoveCategory.Handler handler)
    {
        return await pipeline.Pipe(handler, categoryDto);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eResource claimType, ePermission[] claimValues)>
        {
            (eResource.Category, new[] { ePermission.Remove })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Categories(""), eHttpMethod.Delete, Action, MapAuth);
}
