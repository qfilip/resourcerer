using Resourcerer.Api.Endpoints.Categories;
using Resourcerer.Api.Endpoints.Elements;

namespace Resourcerer.Api.Endpoints;

public class EndpointMapper
{
    public static void Map(WebApplication app)
    {
        var categories = app.MapGroup("/categories");
        categories.MapGet("/getall", GetAllCategoriesEndpoint.Action);

        //var elements = app.MapGroup("/elements");
        //elements.MapGet("/elements-overviews", GetAllElementsOverviewsEndpoint.Action);
    }
}
