using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Api.Services.StaticServices;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Models;
using Resourcerer.Identity.Services;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class RegisterEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1Register dto,
        [FromServices] Pipeline pipeline,
        [FromServices] Register.Handler handler,
        [FromServices] JwtTokenService jwtTokenService)
    {
        return await pipeline.Pipe(handler, dto, (result) =>
        {
            var identity = new AppIdentity(
                result.Id,
                result.Name!,
                result.Email!,
                result.IsAdmin,
                result.CompanyId);

            var jwt = jwtTokenService.GenerateToken(
                identity,
                result.PermissionsMap!,
                result.DisplayName!,
                result.Company!.Name!);

            return Results.Ok(jwt);
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Users("register"), eHttpMethod.Post, Action);
}
