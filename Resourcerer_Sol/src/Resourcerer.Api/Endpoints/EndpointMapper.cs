using Microsoft.AspNetCore.Authentication;
using Resourcerer.Api.Endpoints.Categories;
using Resourcerer.Api.Endpoints.Elements;
using Resourcerer.Api.Endpoints.Mocks;
using Resourcerer.Api.Endpoints.Prices;
using Resourcerer.Api.Endpoints.UnitsOfMeasure;
using Resourcerer.Api.Endpoints.Users;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
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
        MapUsers(app);
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

    private static void MapUsers(WebApplication app)
    {
        var g = GetGroup(app, "Users");
        LoginEndpoint.MapToGroup(g);
        SetPermissionsEndpoint.MapToGroup(g);
    }

    public static void AddAuthorization(
        RouteHandlerBuilder route,
        List<(string claimType, string[] claimValues)> claims)
    {
        if(true)
        {
            route.RequireAuthorization(cfg => 
                claims.ForEach(c => cfg.RequireClaim(c.claimType, c.claimValues)));
        }
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
