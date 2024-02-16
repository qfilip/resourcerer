﻿namespace Resourcerer.Api.Endpoints.V1_0;

public class EndpointMapperV1_0
{
    private const string Version = "1.0";
    public static void Map(WebApplication app)
    {
        MapCategories(app);
        MapEvents(app);
        MapMocks(app);
        MapItems(app);
        MapUnitsOfMeasure(app);
        MapUsers(app);
    }

    private static void MapCategories(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Categories");

        GetAllCategoriesEndpoint.MapToGroup(g);
        CreateCategoryEndpoint.MapToGroup(g);
        RemoveCategoryEndpoint.MapToGroup(g);
    }

    private static void MapEvents(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Events");
        
        CreateItemOrderCancelledEventEndpoint.MapToGroup(g);
        CreateItemDeliveredEventEndpoint.MapToGroup(g);
        CreateItemDiscardedEventEndpoint.MapToGroup(g);
        CreateInstanceOrderedEventEndpoint.MapToGroup(g);
        // CreateItemSellCancelledEventEndpoint.MapToGroup(g);
    }

    private static void MapItems(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Items");

        ChangeItemPriceEndpoint.MapToGroup(g);
        CreateCompositeItemEndpoint.MapToGroup(g);
        CreateElementItemEndpoint.MapToGroup(g);
        GetItemsStatisticsEndpoint.MapToGroup(g);
    }

    private static void MapMocks(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Mocks");
        SeedDatabaseEndpoint.MapToGroup(g);
    }

    private static void MapUnitsOfMeasure(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Units Of Measure");
        CreateUnitOfMeasureEndpoint.MapToGroup(g);
    }

    private static void MapUsers(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Users");
        
        GetAllUsersEndpoint.MapToGroup(g);
        GetUserEndpoint.MapToGroup(g);
        LoginEndpoint.MapToGroup(g);
        RefreshSessionEndpoint.MapToGroup(g);
        RegisterEndpoint.MapToGroup(g);
        SetPermissionsEndpoint.MapToGroup(g);
    }
}