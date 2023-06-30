﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
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
        var endpoint = group.MapPost("/add", Action);
        
        EndpointMapper.AddAuthorization(endpoint, new List<(string claimType, string[] claimValues)>
        {
            (nameof(Category), new[] { ePermission.Write.ToString() })
        });
    }
}
