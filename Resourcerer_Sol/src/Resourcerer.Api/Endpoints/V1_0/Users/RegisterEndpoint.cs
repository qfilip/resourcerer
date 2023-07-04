using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Users;

namespace Resourcerer.Api.Endpoints.V1_0.Users;

public static class RegisterEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] AppUserDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] Register.Handler handler)
    {
        return await pipeline.Pipe<AppUserDto, AppUserDtoValidator, string>(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapPost("/register", Action);
    }
}
