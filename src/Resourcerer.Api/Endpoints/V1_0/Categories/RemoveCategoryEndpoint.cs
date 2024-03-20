using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.V1_0;

namespace Resourcerer.Api.Endpoints.V1_0;

public class RemoveCategoryEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] CategoryDto categoryDto,
        [FromServices] Pipeline pipeline,
        [FromServices] RemoveCategory.Handler handler)
    {
        return await pipeline.Pipe(handler, categoryDto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapDelete("", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Category, new[] { ePermission.Remove })
        });
    }
}
