﻿using Resourcerer.Api.Endpoints.V1_0.Categories;
using Resourcerer.Api.Endpoints.V1_0.Composites;
using Resourcerer.Api.Endpoints.V1_0.Elements;
using Resourcerer.Api.Endpoints.V1_0.Mocks;
using Resourcerer.Api.Endpoints.V1_0.Prices;
using Resourcerer.Api.Endpoints.V1_0.UnitsOfMeasure;
using Resourcerer.Api.Endpoints.V1_0.Users;

namespace Resourcerer.Api.Endpoints.V1_0;

public class EndpointMapperV1_0
{
    private const string Version = "1.0";
    public static void MapV1_0(WebApplication app)
    {
        MapCategories(app);
        MapComposites(app);
        MapElements(app);
        MapMocks(app);
        MapPrices(app);
        MapUnitsOfMeasure(app);
        MapUsers(app);
    }

    private static void MapCategories(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Categories");

        GetAllCategoriesEndpoint.MapToGroup(g);
        CreateCategoryEndpoint.MapToGroup(g);
    }

    private static void MapComposites(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Composites");

        CreateCompositeEndpoint.MapToGroup(g);
    }

    private static void MapElements(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Elements");

        GetAllElementsStatisticsEndpoint.MapToGroup(g);
        CreateElementEndpoint.MapToGroup(g);
    }

    private static void MapMocks(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Mocks");
        SeedDatabaseEndpoint.MapToGroup(g);
    }

    private static void MapPrices(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Prices");
        ChangePriceEndpoint.MapToGroup(g);
    }

    private static void MapUnitsOfMeasure(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Units Of Measure");
        CreateUnitOfMeasureEndpoint.MapToGroup(g);
    }

    private static void MapUsers(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Users");
        LoginEndpoint.MapToGroup(g);
        SetPermissionsEndpoint.MapToGroup(g);
    }
}
