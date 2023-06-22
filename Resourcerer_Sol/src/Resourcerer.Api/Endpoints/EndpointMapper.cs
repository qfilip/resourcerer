﻿using Resourcerer.Api.Endpoints.Categories;
using Resourcerer.Api.Endpoints.Elements;
using Resourcerer.Api.Endpoints.Mocks;
using Resourcerer.Api.Endpoints.UnitsOfMeasure;

namespace Resourcerer.Api.Endpoints;

public class EndpointMapper
{
    public static void Map(WebApplication app)
    {
        var categories = app.MapGroup("/categories");
        categories.MapGet("/getall", GetAllCategoriesEndpoint.Action);
        categories.MapPost("/add", AddCategoryEndpoint.Action);

        var elements = app.MapGroup("/elements");
        elements.MapGet("/elements-overviews", GetAllElementsOverviewsEndpoint.Action);
        elements.MapPost("/add", AddElementEndpoint.Action);

        var unitsOfMeasure = app.MapGroup("unitsofmeasure");
        unitsOfMeasure.MapPost("/add", AddUnitOfMeasureEndpoint.Action);

        var mocks = app.MapGroup("/mocks");
        elements.MapGet("/seed-db", SeedDatabaseEndpoint.Action);
    }
}
