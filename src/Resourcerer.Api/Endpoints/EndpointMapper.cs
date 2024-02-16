﻿using Resourcerer.Api.Endpoints.V1_0;
using Resourcerer.Dtos;
using System.Text.RegularExpressions;

namespace Resourcerer.Api.Endpoints;

public static class EndpointMapper
{
    public static void Map(WebApplication app)
    {
        EndpointMapperV1_0.Map(app);
    }

    public static void AddAuthorization(
        RouteHandlerBuilder route,
        List<(ePermissionSection claimType, ePermission[] claimValues)>? claims = null)
    {
        var authDisabled = true;
        if(authDisabled)
        {
            return;
        }

        if (claims != null)
        {
            route.RequireAuthorization(cfg =>
                claims.ForEach(c =>
                {
                    var cType = c.claimType.ToString();
                    var cValues = c.claimValues.Select(x => x.ToString());
                    cfg.RequireClaim(cType, cValues);
                }));
        }
        else
        {
            route.RequireAuthorization(cfg => cfg.RequireAuthenticatedUser());
        }
    }

    public static RouteGroupBuilder GetGroup(WebApplication app, string version, string name)
    {
        // remove all whitespace
        var path = Regex.Replace(name.ToLower(), @"\s+", "");
        var group = app.MapGroup($"api/{version}/{path}");
        group.WithTags(name);

        return group;
    }
}