using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints;

public static class CreateElementItemLoadFormEndpoint
{
    public static async Task<IResult> Action(
       Guid companyId,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateElementItemLoadForm.Handler handler)
    {
        return await pipeline.Pipe(handler, companyId);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("/create/element/form", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Item, new[] { ePermission.Write })
        });
    }
}
