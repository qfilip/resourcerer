using Microsoft.AspNetCore.Mvc;
using Resourcerer.Dtos.Categories;
using Resourcerer.Logic.Commands.Categories;

namespace Resourcerer.Api.Endpoints.Categories;

public class AddCategoryEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] CategoryDto categoryDto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateCategory.Handler handler)
    {
        return await pipeline.Pipe(handler, categoryDto, nameof(CreateCategory));
    }
}
