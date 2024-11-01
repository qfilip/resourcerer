using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class GetUserEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromQuery] Guid userId,
        [FromServices] Pipeline pipeline,
        [FromServices] GetUser.Handler handler)
    {
        return await pipeline.Pipe(handler, userId);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.User, new[] { ePermission.View })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Users("one"), eHttpMethod.Get, Action, MapAuth);
}
