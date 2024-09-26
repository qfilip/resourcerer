using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Api.Services.StaticServices;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class LoginEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] AppUserDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] Login.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, (result) =>
        {
            var jwt = JwtService.GenerateToken(result);
            return Results.Ok(jwt);
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Users("login"), eHttpMethod.Post, Action);
}
