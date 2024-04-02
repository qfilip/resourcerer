using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Categories;

namespace Resourcerer.Api.Endpoints.V1;

public class CreateCategoryEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1CreateCategory categoryDto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateCategory.Handler handler)
    {
        return await pipeline.Pipe(handler, categoryDto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Category, new[] { ePermission.Write })
        });
    }
}
