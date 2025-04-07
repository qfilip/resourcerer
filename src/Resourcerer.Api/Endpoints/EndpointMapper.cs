using Resourcerer.Api.Services.StaticServices;
using Resourcerer.Identity.Enums;

namespace Resourcerer.Api.Endpoints;

public static class EndpointMapper
{
    public static string Examples(string path) => $"/examples{path}";
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

        MapAllEndponts(serviceTypes, app);
    }

    public static void AddAuthorization(
        RouteHandlerBuilder route,
        List<(eResource claimType, eAction[] claimValues)>? claims = null)
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

    private static void MapAllEndponts(List<AppEndpoint> endpoints, WebApplication app)
    {
        // check name duplicates
        endpoints.ForEach(e =>
        {
            var count = endpoints.Count(x =>
                x.Path == e.Path &&
                x.Major == e.Major &&
                x.Minor == e.Minor &&
                x.Method == e.Method);

            if(count > 1)
            {
                var message = $"More than one endpoint found with path {e.Path} and method {e.Method}";
                throw new InvalidOperationException(message);
            }
        });

        var apiVersions = app.NewApiVersionSet();

        var apiVersionSet = apiVersions.ReportApiVersions().Build();

        var groupPrefixes = endpoints
            .Select(x => x.Path.Split('/')[0])
            .DistinctBy(x => x)
            .ToArray();

        foreach(var groupPrefix in groupPrefixes)
        {
            var group = app
                .MapGroup("v{version:apiVersion}" + $"/{groupPrefix}") // important, otherwise not working
                .WithApiVersionSet(apiVersionSet);

            var groupEndpoints = endpoints
                .Where(x => x.Path.Split('/')[0] == groupPrefix)
                .ToArray();

            if(groupEndpoints.Length == 0)
                throw new InvalidOperationException($"No endpoints found for group {groupPrefix}");

            foreach(var e in groupEndpoints)
            {
                var endpointPathParts = e.Path
                .Split('/')
                .Skip(1);

                var endpointPath = string.Join("/", endpointPathParts);

                var endpoint = e.Method switch
                {
                    eHttpMethod.Get => group.MapGet(endpointPath, e.EndpointAction),
                    eHttpMethod.Put => group.MapPut(endpointPath, e.EndpointAction),
                    eHttpMethod.Patch => group.MapPatch(endpointPath, e.EndpointAction),
                    eHttpMethod.Post => group.MapPost(endpointPath, e.EndpointAction),
                    eHttpMethod.Delete => group.MapDelete(endpointPath, e.EndpointAction),
                    _ => throw new InvalidOperationException($"HttpMethod {e.Method} not supported")
                };

                endpoint
                    .MapToApiVersion(e.Major, e.Minor);

                if (AppStaticData.Auth.Enabled)
                    e.MapAuth?.Invoke(endpoint);
            }
        }
    }
}
