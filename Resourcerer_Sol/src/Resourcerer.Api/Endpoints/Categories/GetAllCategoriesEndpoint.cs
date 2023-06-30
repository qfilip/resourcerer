using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Queries.Categories;

namespace Resourcerer.Api.Endpoints.Categories;

public class GetAllCategoriesEndpoint
{
    public static async Task<IResult> Action(
        [FromServices] Pipeline pipeline,
        [FromServices] GetAllCategories.Handler handler)
    {
        return await pipeline.Pipe(handler, new Unit());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("", Action);
        
        EndpointMapper.AddAuthorization(endpoint, new List<(string claimType, string[] claimValues)>
        {
            (nameof(Category), new[] { ePermission.Read.ToString() })
        });
    }
}