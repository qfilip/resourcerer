using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Commands.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class SetPermissionsEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1SetUserPermissions dto,
       [FromServices] Pipeline pipeline,
       [FromServices] SetPermissions.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/permissions", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.User, new[] { ePermission.Write })
        });
    }
}
