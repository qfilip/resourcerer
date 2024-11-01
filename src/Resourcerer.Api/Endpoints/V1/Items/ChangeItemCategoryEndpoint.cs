using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class ChangeItemCategoryEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1ChangeItemCategory dto,
       [FromServices] Pipeline pipeline,
       [FromServices] ChangeItemCategory.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Item, new[] { ePermission.Update })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Items("category"), eHttpMethod.Post, Action, MapAuth);
}
