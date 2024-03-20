using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.V1.Commands;

namespace Resourcerer.Api.Endpoints.V1;

public static class RegisterEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] AppUserDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] Register.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, (result) =>
        {
            var jwt = JwtService.GenerateToken(result);
            return Results.Ok(jwt);
        });
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapPost("/register", Action);
    }
}
