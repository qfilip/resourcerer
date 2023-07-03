using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Prices;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Elements;

namespace Resourcerer.Api.Endpoints.V1_0.Elements;

public class ChangeElementPriceEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] ChangePriceDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] ChangeElementPrice.Handler handler)
    {
        return await pipeline
            .Pipe<ChangePriceDto, ChangePriceDtoValidator, Unit>(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/change-price", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(string claimType, string[] claimValues)>
        {
            (nameof(Element), new[] { ePermission.Write.ToString() })
        });
    }
}
