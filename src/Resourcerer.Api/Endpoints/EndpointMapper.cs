using Resourcerer.Api.Endpoints.Fake;
using Resourcerer.Api.Endpoints.V1;
using Resourcerer.Api.Services.StaticServices;
using Resourcerer.Dtos;
using System.Text.RegularExpressions;

namespace Resourcerer.Api.Endpoints;

public static class EndpointMapper
{
    public static string Fake(string path) => $"fake/{path}";
    public static string Categories(string path) => $"categories/{path}";
    public static string Companies(string path) => $"companies/{path}";
    public static string Instances(string path) => $"instances/{path}";
    public static string Items(string path) => $"items/{path}";
    public static string UnitsOfMeasure(string path) => $"unitsOfMeasure/{path}";
    public static string Users(string path) => $"users/{path}";

    public static void Map(WebApplication app)
    {
        var assembly = typeof(Api.IAssemblyMarker).Assembly;

        var serviceTypes = assembly
        .GetTypes()
        .Where(x =>
            x.GetInterface(typeof(IAppEndpoint).Name) != null &&
            !x.IsAbstract &&
            !x.IsInterface)
        .Select(x =>
        {
            var endpoint = Activator.CreateInstance(x) as IAppEndpoint;
            return endpoint!.GetEndpointInfo();
        })
        .ToList();

        AppEndpoint.MapAllVersions(serviceTypes, app);
    }

    public static void AddAuthorization(
        RouteHandlerBuilder route,
        List<(ePermissionSection claimType, ePermission[] claimValues)>? claims = null)
    {
        if(!AppStaticData.Auth.Enabled) // set in appsetting.json
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
