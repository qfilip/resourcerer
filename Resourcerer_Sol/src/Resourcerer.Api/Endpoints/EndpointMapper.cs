using Resourcerer.Api.Endpoints.V1_0;
using System.Text.RegularExpressions;

namespace Resourcerer.Api.Endpoints;

public static class EndpointMapper
{
    public static void Map(WebApplication app)
    {
        EndpointMapperV1_0.MapV1_0(app);
    }

    public static void AddAuthorization(
        RouteHandlerBuilder route,
        List<(string claimType, string[] claimValues)> claims)
    {
        if (true)
        {
            route.RequireAuthorization(cfg =>
                claims.ForEach(c => cfg.RequireClaim(c.claimType, c.claimValues)));
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
