using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

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
        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.User, new[] { ePermission.Read })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Users("one"), HttpMethod.Get, Action, MapAuth);
}
