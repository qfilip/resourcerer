using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.Composites;

namespace Resourcerer.Api.Endpoints.V1_0.Composites;

public class CreateCompositeEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] CreateCompositeDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateComposite.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Composite, new[] { ePermission.Write })
        });
    }
}
