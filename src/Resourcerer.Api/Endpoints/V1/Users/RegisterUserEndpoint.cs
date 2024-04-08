using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class RegisterUserEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1RegisterUser dto,
        [FromServices] Pipeline pipeline,
        [FromServices] RegisterUser.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/register-user", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.User, new[] { ePermission.Write })
        });
    }
}
