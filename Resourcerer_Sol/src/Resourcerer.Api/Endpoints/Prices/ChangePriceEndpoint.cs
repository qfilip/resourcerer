using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Prices;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Prices;

namespace Resourcerer.Api.Endpoints.Prices;

public class ChangePriceEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] PriceDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] ChangePrice.Handler handler)
    {
        return await pipeline.Pipe<PriceDto, PriceDtoValidator, Unit>(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/change", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(string claimType, string[] claimValues)>
        {
            (nameof(Price), new[] { ePermission.Write.ToString() })
        });
    }
}
