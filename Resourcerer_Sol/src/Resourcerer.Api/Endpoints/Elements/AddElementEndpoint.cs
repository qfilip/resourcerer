using Microsoft.AspNetCore.Mvc;
using Resourcerer.Dtos.Elements;
using Resourcerer.Logic.Categories.Commands;
using Resourcerer.Logic.Elements.Commands;

namespace Resourcerer.Api.Endpoints.Elements;

public class AddElementEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] ElementDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] AddElement.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, nameof(AddElement));
    }
}
