using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.Categories;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Categories;

namespace Resourcerer.Api.Endpoints.Categories;

public class AddCategoryEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] CategoryDto categoryDto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateCategory.Handler handler)
    {   
        return await pipeline
            .Pipe<CategoryDto, CategoryDtoValidator, Unit>(handler, categoryDto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapPost("/add", Action);
    }
}
