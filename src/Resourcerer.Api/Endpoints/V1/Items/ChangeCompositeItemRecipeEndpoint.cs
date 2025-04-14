using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class ChangeCompositeItemRecipeEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1ChangeCompositeItemRecipe dto,
       [FromServices] Pipeline pipeline,
       [FromServices] ChangeCompositeItemRecipe.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eResource claimType, ePermission[] claimValues)>
        {
            (eResource.Item, new[] { ePermission.Update })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Items("recipe"), eHttpMethod.Post, Action, MapAuth);
}
