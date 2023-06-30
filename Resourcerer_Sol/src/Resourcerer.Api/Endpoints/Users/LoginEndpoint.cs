﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.Users;
using Resourcerer.Logic.Queries.Users;

namespace Resourcerer.Api.Endpoints.Users;

public class LoginEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] UserDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] Login.Handler handler)
    {
        return await pipeline.Pipe<UserDto, UserDtoValidator, UserDto>(handler, dto, (result) =>
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
