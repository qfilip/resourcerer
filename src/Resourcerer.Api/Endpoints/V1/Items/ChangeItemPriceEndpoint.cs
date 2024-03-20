using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Commands.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class ChangeItemPriceEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] V1ChangePrice dto,
       [FromServices] Pipeline pipeline,
       [FromServices] ChangeItemPrice.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/price", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
