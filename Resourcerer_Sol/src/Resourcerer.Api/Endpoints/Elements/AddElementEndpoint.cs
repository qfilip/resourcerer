using Microsoft.AspNetCore.Mvc;
using Resourcerer.Dtos.Elements;
using Resourcerer.Logic.Commands.Elements;

namespace Resourcerer.Api.Endpoints.Elements;

public class AddElementEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] ElementDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateElement.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, nameof(CreateElement));
    }
}
