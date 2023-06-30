﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Logic;
using Resourcerer.Logic.Queries.Categories;

namespace Resourcerer.Api.Endpoints.Categories;

public class GetAllCategoriesEndpoint
{
    public static async Task<IResult> Action(
        [FromServices] Pipeline pipeline,
        [FromServices] GetAllCategories.Handler handler)
    {
        return await pipeline.Pipe(handler, new Unit());
    }
}