using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.V1;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class ChangeCompanyNameEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] V1ChangeCompanyName dto,
        [FromServices] Pipeline pipeline,
        [FromServices] ChangeCompanyName.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Company, new[] { ePermission.Modify })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0, EndpointMapper.Companies("name"), eHttpMethod.Post, Action, MapAuth);
}
