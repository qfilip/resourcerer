using Resourcerer.Api.Services.StaticServices;
using Resourcerer.Identity.Enums;

namespace Resourcerer.Api.Endpoints;

public static class EndpointMapper
{
    // versioning sample at the bottom
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

        MapAllVersions(x => x.Path.Split("/")[0], serviceTypes, app);
    }

    public static void AddAuthorization(
        RouteHandlerBuilder route,
        List<(eResource claimType, ePermission[] claimValues)>? claims = null)
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

    /// <summary>
    /// Auto generate newer versions of endpoints that haven't changed.
    /// </summary>
    /// <param name="endpoints">Endpoints to use.</param>
    /// <param name="apiVersionSetBuilder">ApiVersionSetBuilder to map versions to.</param>
    /// <returns></returns>
    public static void MapAllVersions(
        Func<AppEndpoint, string> resourceSelector,
        List<AppEndpoint> endpoints,
        WebApplication app)
    {
        // check name duplicates
        endpoints.ForEach(e =>
        {
            var count = endpoints.Count(x =>
                x.Path == e.Path &&
                x.Major == e.Major &&
                x.Minor == e.Minor &&
                x.Method == e.Method);

            if (count > 1)
            {
                var message = $"More than one endpoint found with path {e.Path} and method {e.Method}";
                throw new InvalidOperationException(message);
            }
        });

        var resources = endpoints
            .Select(resourceSelector)
            .Distinct()
            .ToArray();

        var majors = endpoints.Select(x => x.Major).Distinct().ToArray();

        var mapped = new List<AppEndpoint>();

        foreach(var major in majors)
        {
            foreach (var resource in resources)
            {
                var minors = endpoints
                    .Where(x => x.Major == major)
                    .Select(x => x.Minor)
                    .Distinct()
                    .ToArray();

                var resourceEndpoints = endpoints
                    .Where(x => x.Path.StartsWith(resource))
                    .GroupBy(x => new { x.Path, x.Method })
                    .ToArray();

                foreach (var re in resourceEndpoints)
                {
                    var endpointVersions = re.ToArray();

                    var minimumMajor = endpointVersions.Min(x => x.Major);
                    
                    if (minimumMajor > major)
                        continue;

                    foreach (var minor in minors)
                    {
                        var version = endpointVersions
                            .FirstOrDefault(x =>
                                x.Major == major &&
                                x.Minor == minor
                            );

                        if (version != null)
                        {
                            mapped.Add(version);
                        }
                        else
                        {
                            var last = endpointVersions
                                .OrderBy(x => x.Major)
                                .ThenBy(x => x.Minor)
                                .Last();

                            mapped.Add(last with { Major = major, Minor = minor });
                        }
                    }
                }
            }
        }

        // map version sets
        var apiVersionSetBuilder = app.NewApiVersionSet();

        foreach (var major in majors)
        {
            var minors = mapped
                .Where(x => x.Major == major)
                .Select(x => x.Minor)
                .Distinct()
                .Order()
                .ToArray();

            foreach(var minor in minors)
                apiVersionSetBuilder.HasApiVersion(new Asp.Versioning.ApiVersion(major, minor));
        }

        mapped.ForEach(e =>
        {
            var path = $"v{e.Major}.{e.Minor}/{e.Path}";
            var endpoint = e.Method switch
            {
                eHttpMethod.Get => app.MapGet(path, e.EndpointAction),
                eHttpMethod.Put => app.MapPut(path, e.EndpointAction),
                eHttpMethod.Patch => app.MapPatch(path, e.EndpointAction),
                eHttpMethod.Post => app.MapPost(path, e.EndpointAction),
                eHttpMethod.Delete => app.MapDelete(path, e.EndpointAction),
                _ => throw new InvalidOperationException($"HttpMethod {e.Method} not supported")
            };
            
            PrintEndpointToConsole(e.Method, path);
        });

        Console.WriteLine($"Endpoint count: {mapped.Count}");
    }

    private static void PrintEndpointToConsole(eHttpMethod method, string path)
    {
        var space = 7 - method.ToString().Length;
        Console.Write($"Mapped {method}");
        
        for (int i = 0; i < space; i++)
            Console.Write(" ");

        Console.Write($"/{path}{Environment.NewLine}");
    }
}
