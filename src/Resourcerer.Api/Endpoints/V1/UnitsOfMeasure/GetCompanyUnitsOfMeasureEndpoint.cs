using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class GetCompanyUnitsOfMeasureEndpoint
{
    public static async Task<IResult> Action(
       [FromQuery] Guid companyId,
       [FromServices] Pipeline pipeline,
       [FromServices] GetCompanyUnitsOfMeasure.Handler handler)
    {
        return await pipeline.Pipe(handler, companyId);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("/company-all", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Company, new[] { ePermission.Read })
        });
    }
}
