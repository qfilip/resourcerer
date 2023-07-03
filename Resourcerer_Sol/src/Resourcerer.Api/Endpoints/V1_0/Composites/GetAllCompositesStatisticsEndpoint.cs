using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Queries.Composites;

namespace Resourcerer.Api.Endpoints.V1_0.Composites;

public class GetAllCompositesStatisticsEndpoint
{
    public static async Task<IResult> Action(
        [FromServices] Pipeline pipeline,
        [FromServices] GetAllCompositesStatistics.Handler handler)
    {
        return await pipeline.Pipe(handler, new Unit());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("/statistics", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(string claimType, string[] claimValues)>
        {
            (nameof(Composite), new[] { ePermission.Read.ToString() })
        });
    }
}
