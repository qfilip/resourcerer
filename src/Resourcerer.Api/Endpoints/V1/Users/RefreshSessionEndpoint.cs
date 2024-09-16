using Resourcerer.Api.Services.StaticServices;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Resourcerer.Api.Endpoints.V1;

public class RefreshSessionEndpoint : IAppEndpoint
{
    public static IResult Action(HttpContext context)
    {
        var claims = context.User.Claims;
        var token = JwtService.RefreshToken(claims);

        return Results.Ok(token);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint);
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Users("refresh_session"), HttpMethod.Get, Action, MapAuth);
}