using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.V1_0;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateUnitOfMeasureEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] CreateUnitOfMeasureDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateUnitOfMeasure.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Element, new[] { ePermission.Write })
        });
    }
}
