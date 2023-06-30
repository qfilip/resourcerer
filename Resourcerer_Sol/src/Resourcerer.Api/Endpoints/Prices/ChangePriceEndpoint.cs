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
        group.MapPost("/change", Action)
            .RequireAuthorization(cfg =>
            {
                cfg.RequireClaim(nameof(Price), ePermission.Write.ToString());
            });
    }
}
