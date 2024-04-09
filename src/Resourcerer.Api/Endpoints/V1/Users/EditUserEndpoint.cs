using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class EditUserEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1EditUser dto,
        [FromServices] Pipeline pipeline,
        [FromServices] EditUser.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/edit", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.User, new[] { ePermission.Modify })
        });
    }
}
