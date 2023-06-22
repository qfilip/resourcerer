using Microsoft.AspNetCore.Mvc;
using Resourcerer.Dtos.Categories;
using Resourcerer.Logic.Categories.Commands;

namespace Resourcerer.Api.Endpoints.Categories;

public class AddCategoryEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] CategoryDto categoryDto,
        [FromServices] Pipeline pipeline,
        [FromServices] AddCategory.Handler handler)
    {
        return await pipeline.Pipe(handler, categoryDto, nameof(AddCategory));
    }
}
