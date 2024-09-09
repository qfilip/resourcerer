using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class GetElementItemForEditEndpoint
{
    public static async Task<IResult> Action(
        [FromQuery] Guid itemId,
        [FromQuery] Guid companyId,
        Pipeline pipeline,
        GetElementItemForEdit.Handler handler)
    {
        return await pipeline.Pipe(handler, (itemId, companyId));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("/edit/element/form", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Item, new[] { ePermission.Read })
        });
    }
}
