using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Api.Services.StaticServices;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class RegisterEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1Register dto,
        [FromServices] Pipeline pipeline,
        [FromServices] Register.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, (result) =>
        {
            var jwt = JwtService.GenerateToken(result);
            return Results.Ok(jwt);
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Users("register"), eHttpMethod.Post, Action);
}
