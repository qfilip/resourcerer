using Asp.Versioning.Builder;
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

        MapAllVersions(serviceTypes, app);
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

    private static void MapAllVersions(List<AppEndpoint> endpoints, WebApplication app)
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

        endpoints = MapAllEndpointVersions(endpoints, apiVersions);

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

    /// <summary>
    /// Auto generate newer versions of endpoints that haven't changed.
    /// </summary>
    /// <param name="endpoints">Endpoints to use.</param>
    /// <param name="apiVersionSetBuilder">ApiVersionSetBuilder to map versions to.</param>
    /// <returns></returns>
    private static List<AppEndpoint> MapAllEndpointVersions(List<AppEndpoint> endpoints, ApiVersionSetBuilder apiVersionSetBuilder)
    {
        var lookup = new Dictionary<int, (int min, int max)>();

        var minMajor = endpoints.Min(x => x.Major);
        var maxMajor = endpoints.Max(x => x.Major);

        var major = minMajor;
        while (major <= maxMajor)
        {
            var minMinor = endpoints.Where(x => x.Major == major).Min(x => x.Minor);
            var maxMinor = endpoints.Where(x => x.Major == major).Max(x => x.Minor);

            lookup.Add(major, (minMinor, maxMinor));

            major++;
        }

        major = endpoints.Min(x => x.Major);

        var collection = new List<AppEndpoint>();
        
        foreach (var e in endpoints)
        {
            var nextMinor = endpoints
                .Where(x =>
                    x.Path == e.Path &&
                    x.Major == e.Major &&
                    x.Minor > e.Minor)
                .OrderBy(x => x.Minor)
                .FirstOrDefault();

            var nextMajor = endpoints
                .Where(x =>
                    x.Path == e.Path &&
                    x.Major > e.Major)
                .OrderBy(x => x.Minor)
                .FirstOrDefault();

            var toMajor = maxMajor;
            var toMinor = lookup[maxMajor].max;

            if(nextMinor != null)
            {
                toMajor = nextMinor.Major;
                toMinor = nextMinor.Minor;
            }
            else if(nextMajor != null)
            {
                toMajor = nextMajor.Major;
                toMinor = nextMajor.Minor;
            }
            
            var eMajor = e.Major;
            var eMinor = e.Minor;

            while (eMajor <= toMajor)
            {
                if(nextMinor != null)
                {
                    while (eMinor < toMinor)
                    {
                        Console.WriteLine($"Mapped [{e.Method}] v{eMajor}.{eMinor}/{e.Path}");

                        collection.Add(new AppEndpoint(eMajor, eMinor, e.Path, e.Method, e.EndpointAction, e.MapAuth));
                        apiVersionSetBuilder.HasApiVersion(new Asp.Versioning.ApiVersion(eMajor, eMinor));
                    
                        eMinor++;
                    }
                }
                else
                {
                    Console.WriteLine($"Mapped [{e.Method}] v{eMajor}.{eMinor}/{e.Path}");
                    collection.Add(new AppEndpoint(eMajor, eMinor, e.Path, e.Method, e.EndpointAction, e.MapAuth));
                    apiVersionSetBuilder.HasApiVersion(new Asp.Versioning.ApiVersion(eMajor, eMinor));
                    break;
                }

                eMajor++;
            }
        }

        return collection
            .DistinctBy(x => new { x.Major, x.Minor, x.Path, x.Method })
            .ToList();
    }

    //var apiVersions = app
    //    .NewApiVersionSet()
    //    .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
    //    .HasApiVersion(new Asp.Versioning.ApiVersion(2, 0))
    //    .ReportApiVersions().Build();

    //var g = app.MapGroup("v{version:apiVersion}").WithApiVersionSet(apiVersions);

    //g.MapGet("/get", () => Results.Ok())
    //    .MapToApiVersion(1, 0);

    //g.MapGet("/get", () => Results.Ok())
    //    .MapToApiVersion(2, 0);
}
