using MediatR;
using Microsoft.AspNetCore.Mvc;
using Resourcerer.Logic.Categories.Queries;

namespace Resourcerer.Api.Endpoints;
public static class Categories
{
    private static IResult CreateCategory()
    {
        return Results.Ok();
    }

    private static async Task<IResult> GetAll(IMediator mediator)
    {
        var categories = await mediator.Send(new GetAllCategories.Query());
        return Results.Ok(categories);
    }

    public static void MapEndpoints(WebApplication app)
    {
        app.MapGet("/categories/all", GetAll);
            //.RequireAuthorization(AppStaticData.AuthPolicy.Admin);

        app.MapPost("/categories/create", CreateCategory)
            .RequireAuthorization(AppStaticData.AuthPolicy.Admin);
    }
}

