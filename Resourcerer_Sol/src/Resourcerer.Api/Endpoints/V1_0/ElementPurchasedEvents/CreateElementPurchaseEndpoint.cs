using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.ElementPurchasedEvents;

namespace Resourcerer.Api.Endpoints.V1_0.ElementPurchaseEvents;

public class CreateElementPurchaseEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] CreateElementPurchaseDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateElementPurchase.Handler handler)
    {
        return await pipeline.Pipe<CreateElementPurchaseDto, CreateElementPurchaseDtoValidator, Unit>
            (handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create-element-purchase", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Element, new[] { ePermission.Write })
        });
    }
}
