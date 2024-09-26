using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items;

namespace Resourcerer.Api.Endpoints.V1;

public class ChangeItemPriceEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1ChangePrice dto,
       [FromServices] Pipeline pipeline,
       [FromServices] ChangeItemPrice.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Item, new[] { ePermission.Modify })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Items("price"), eHttpMethod.Post, Action, MapAuth);
}
