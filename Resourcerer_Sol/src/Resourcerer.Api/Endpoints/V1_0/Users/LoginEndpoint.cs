using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Users;
using Resourcerer.Logic.Queries.Users;

namespace Resourcerer.Api.Endpoints.V1_0.Users;

public class LoginEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] AppUserDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] Login.Handler handler)
    {
        return await pipeline.Pipe<AppUserDto, AppUserDtoValidator, AppUserDto>(handler, dto, (result) =>
        {
            var jwt = JwtService.GenerateToken(result.Claims!);
            return Results.Ok(jwt);
        });
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapPost("/login", Action);
    }
}
