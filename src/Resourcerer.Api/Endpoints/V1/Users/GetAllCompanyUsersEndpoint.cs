﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class GetAllCompanyUsersEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
        [FromQuery] Guid companyId,
        [FromServices] Pipeline pipeline,
        [FromServices] GetAllCompanyUsers.Handler handler)
    {
        return await pipeline.Pipe(handler, companyId);
    }

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.User, new[] { ePermission.Read })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Users("company-all"), eHttpMethod.Get, Action, MapAuth);
}
