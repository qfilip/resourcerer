using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1;
using Resourcerer.Logic.V1.Items;

namespace Resourcerer.Api.Endpoints.V1;

public class GetCompanyItemsEndpoint
{
    public static async Task<IResult> Action(
        Guid companyId,
        Pipeline pipeline,
        GetCompanyItems.Handler handler)
    {
        return await pipeline.Pipe(handler, companyId);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("/company-all", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Item, new[] { ePermission.Read })
        });
    }
}
