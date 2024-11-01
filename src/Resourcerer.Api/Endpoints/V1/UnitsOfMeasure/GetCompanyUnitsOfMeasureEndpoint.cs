using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class GetCompanyUnitsOfMeasureEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromQuery] Guid companyId,
       [FromServices] Pipeline pipeline,
       [FromServices] GetCompanyUnitsOfMeasure.Handler handler)
    {
        return await pipeline.Pipe(handler, companyId);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Company, new[] { ePermission.View })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.UnitsOfMeasure("company-all"), eHttpMethod.Get, Action, MapAuth);
}
