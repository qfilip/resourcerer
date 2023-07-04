using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Users;

namespace Resourcerer.Api.Endpoints.V1_0.Users;

public class SetPermissionsEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] SetUserPermissionsDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] SetPermissions.Handler handler)
    {
        return await
            pipeline
            .Pipe<SetUserPermissionsDto, SetUserPermissionsDtoValidator, Unit>(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/set-permissions", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(string claimType, string[] claimValues)>
        {
            (nameof(AppUser), new[] { ePermission.Write.ToString() })
        });
    }
}
