using Microsoft.AspNetCore.Mvc;
using Resourcerer.Dtos.Prices;
using Resourcerer.Logic.Prices.Commands;

namespace Resourcerer.Api.Endpoints.Prices;

public class ChangePriceEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] PriceDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] ChangePrice.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, nameof(ChangePrice));
    }
}
