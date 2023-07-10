using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.ElementEvents;

namespace Resourcerer.Api.Endpoints.V1_0.ElementEvents;

public class CreateElementPurchasedEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] CreateElementPurchasedEventDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateElementPurchasedEvent.Handler handler)
    {
        return await pipeline.Pipe<CreateElementPurchasedEventDto, CreateElementPurchaseDtoValidator, Unit>
            (handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create-element-purchased", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Element, new[] { ePermission.Write })
        });
    }
}
