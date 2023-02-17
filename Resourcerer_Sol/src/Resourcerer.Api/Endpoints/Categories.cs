namespace Resourcerer.Api.Endpoints;
public static class Categories
{
    private static IResult CreateCategory()
    {
        return Results.Ok();
    }

    public static void MapEndpoints(WebApplication app)
    {
        app.MapPost("category/", CreateCategory)
            .RequireAuthorization(AppStaticData.AuthPolicy.Admin);
    }
}

