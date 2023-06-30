using Resourcerer.Api.Endpoints.Categories;
using Resourcerer.Api.Endpoints.Elements;
using Resourcerer.Api.Endpoints.Mocks;
using Resourcerer.Api.Endpoints.Prices;
using Resourcerer.Api.Endpoints.UnitsOfMeasure;
using System.Text.RegularExpressions;

namespace Resourcerer.Api.Endpoints;

public class EndpointMapper
{
    public static void Map(WebApplication app)
    {
        MapCategories(app);
        MapElements(app);
        MapMocks(app);
        MapPrices(app);
        MapUnitsOfMeasure(app);
    }

    private static void MapCategories(WebApplication app)
    {
        var g = GetGroup(app, "Categories");

        GetAllCategoriesEndpoint.MapToGroup(g);
        AddCategoryEndpoint.MapToGroup(g);
    }

    private static void MapElements(WebApplication app)
    {
        var g = GetGroup(app, "Elements");

        GetAllElementsOverviewsEndpoint.MapToGroup(g);
        AddElementEndpoint.MapToGroup(g);
        g.RequireAuthorization(cfg =>
        {
            cfg.RequireClaim("elements");
        });
    }

    private static void MapMocks(WebApplication app)
    {
        var g = GetGroup(app, "Mocks");
        SeedDatabaseEndpoint.MapToGroup(g);
    }

    private static void MapPrices(WebApplication app)
    {
        var g = GetGroup(app, "Prices");
        ChangePriceEndpoint.MapToGroup(g);
    }

    private static void MapUnitsOfMeasure(WebApplication app)
    {
        var g = GetGroup(app, "Units Of Measure");
        AddUnitOfMeasureEndpoint.MapToGroup(g);
    }

    private static RouteGroupBuilder GetGroup(WebApplication app, string name)
    {
        // remove all whitespace
        var path = Regex.Replace(name.ToLower(), @"\s+", "");
        var group = app.MapGroup($"{path}");
        group.WithTags(name);

        return group;
    }
}
