using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class RegisterUserEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1RegisterUser dto,
        [FromServices] Pipeline pipeline,
        [FromServices] RegisterUser.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eResource claimType, ePermission[] claimValues)>
        {
            (eResource.User, new[] { ePermission.Create })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Users("register-user"), eHttpMethod.Post, Action, MapAuth);
}
