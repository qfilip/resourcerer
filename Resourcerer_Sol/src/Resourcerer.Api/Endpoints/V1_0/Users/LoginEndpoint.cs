using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Queries.Users;

namespace Resourcerer.Api.Endpoints.V1_0.Users;

public class LoginEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] AppUserDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] Login.Handler handler)
    {
        return await pipeline.PipeWithValidator(handler, dto, (result) =>
        {
            var jwt = JwtService.GenerateToken(result);
            return Results.Ok(jwt);
        });
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapPost("/login", Action);
    }
}
