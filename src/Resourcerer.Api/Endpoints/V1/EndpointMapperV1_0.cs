using Resourcerer.Api.Endpoints.V1.Items.Production;

namespace Resourcerer.Api.Endpoints.V1;

public class EndpointMapperV1_0
{
    private const string Version = "1.0";
    public static void Map(WebApplication app)
    {
        MapCompanies(app);
        MapCategories(app);
        MapInstances(app);
        MapItems(app);
        MapUnitsOfMeasure(app);
        MapUsers(app);
    }

    private static void MapCompanies(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Companies");

        GetAllCompaniesEndpoint.MapToGroup(g);
        GetCompanyOverviewEndpoint.MapToGroup(g);
    }

    private static void MapCategories(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Categories");

        GetAllCompanyCategoriesEndpoint.MapToGroup(g);
        CreateCategoryEndpoint.MapToGroup(g);
        RemoveCategoryEndpoint.MapToGroup(g);
    }

    private static void MapInstances(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Instances");
        
        CreateInstanceOrderCancelledEventEndpoint.MapToGroup(g);
        CreateInstanceDeliveredEventEndpoint.MapToGroup(g);
        CreateInstanceDiscardedEventEndpoint.MapToGroup(g);
        CreateInstanceOrderedEventEndpoint.MapToGroup(g);
    }

    private static void MapItems(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Items");

        ChangeItemPriceEndpoint.MapToGroup(g);
        CreateCompositeItemEndpoint.MapToGroup(g);
        CreateElementItemEndpoint.MapToGroup(g);
        GetItemsStatisticsEndpoint.MapToGroup(g);

        // production
        CreateItemProductionOrderEndpoint.MapToGroup(g);
        CancelItemProductionOrderEndpoint.MapToGroup(g);
        FinishItemProductionOrderEndpoint.MapToGroup(g);
        StartItemProductionEndpoint.MapToGroup(g);
    }

    private static void MapUnitsOfMeasure(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Units Of Measure");
        
        CreateUnitOfMeasureEndpoint.MapToGroup(g);
        EditUnitOfMeasureEndpoint.MapToGroup(g);
        GetCompanyUnitsOfMeasureEndpoint.MapToGroup(g);
    }

    private static void MapUsers(WebApplication app)
    {
        var g = EndpointMapper.GetGroup(app, Version, "Users");
        
        EditUserEndpoint.MapToGroup(g);
        GetAllCompanyUsersEndpoint.MapToGroup(g);
        GetUserEndpoint.MapToGroup(g);
        LoginEndpoint.MapToGroup(g);
        RefreshSessionEndpoint.MapToGroup(g);
        RegisterEndpoint.MapToGroup(g);
        RegisterUserEndpoint.MapToGroup(g);
        SetPermissionsEndpoint.MapToGroup(g);
    }
}
