using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.V1_0;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateCompositeItemEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] CreateCompositeItemDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateCompositeItem.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, new CreateCompositeItemDto.Validator());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create-composite", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
