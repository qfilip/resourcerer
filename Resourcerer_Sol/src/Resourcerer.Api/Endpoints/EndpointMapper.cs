using Resourcerer.Api.Endpoints.Categories;
using Resourcerer.Api.Endpoints.Elements;
using Resourcerer.Api.Endpoints.Mocks;
using Resourcerer.Api.Endpoints.UnitsOfMeasure;

namespace Resourcerer.Api.Endpoints;

public class EndpointMapper
{
    public static void Map(WebApplication app)
    {
        MapCategories(app);
        MapElements(app);
        MapUnitsOfMeasure(app);
        MapMocks(app);
    }

    private static void MapCategories(WebApplication app)
    {
        var categories = app.MapGroup("/categories");
        categories.MapGet("/getall", GetAllCategoriesEndpoint.Action);
        categories.MapPost("/add", AddCategoryEndpoint.Action);
        categories.WithTags("Categories");
    }

    private static void MapElements(WebApplication app)
    {
        var elements = app.MapGroup("/elements");
        elements.MapGet("/elements-overviews", GetAllElementsOverviewsEndpoint.Action);
        elements.MapPost("/add", AddElementEndpoint.Action);
        elements.WithTags("Elements");
        elements.RequireAuthorization(cfg =>
        {
            cfg.RequireClaim("elements");
        });
    }

    private static void MapUnitsOfMeasure(WebApplication app)
    {
        var unitsOfMeasure = app.MapGroup("unitsofmeasure");
        unitsOfMeasure.MapPost("/add", AddUnitOfMeasureEndpoint.Action);
        unitsOfMeasure.WithTags("Units Of Measure");
    }

    private static void MapMocks(WebApplication app)
    {
        var mocks = app.MapGroup("/mocks");
        mocks.MapGet("/seed-db", SeedDatabaseEndpoint.Action);
        mocks.WithTags("Mocks");
    }
}
