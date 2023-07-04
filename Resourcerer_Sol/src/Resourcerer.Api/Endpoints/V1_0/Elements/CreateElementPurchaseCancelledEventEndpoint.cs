using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.Elements.Events;

namespace Resourcerer.Api.Endpoints.V1_0.Elements;

public class CreateElementPurchaseCancelledEventEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] ElementPurchaseCancelledEventDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateElementPurchaseCancelledEvent.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create-element-purchase-cancelled", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Element, new[] { ePermission.Write })
        });
    }
}
