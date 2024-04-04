using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.V1.Users;

namespace Resourcerer.Api.Endpoints.V1;

public class LoginEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] AppUserDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] Login.Handler handler)
    {
        //return await pipeline.Pipe(handler, dto, (result) =>
        //{
        //    var jwt = JwtService.GenerateToken(result);
        //    return Results.Ok(jwt);
        //});
        return await Task.FromResult(Results.Ok(dto));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapPost("/login", Action);
    }
}
