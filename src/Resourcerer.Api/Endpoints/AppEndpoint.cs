using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Resourcerer.Api.Endpoints;

public record AppEndpoint(
    int Major,
    int Minor,
    string Path,
    HttpMethod Method,
    Delegate EndpointAction,
    Action<RouteHandlerBuilder>? MapAuth)
{
    public static void MapAllVersions(List<AppEndpoint> endpoints, WebApplication app)
    {
        var lookup = new Dictionary<int, (int min, int max)>();

        var minMajor = endpoints.Min(x => x.Major);
        var maxMajor = endpoints.Max(x => x.Major);

        lookup.Add(minMajor, (
            endpoints.Where(x => x.Major == minMajor).Min(x => x.Minor),
            endpoints.Where(x => x.Major == minMajor).Max(x => x.Minor)
        ));

        var major = minMajor;
        while (major < maxMajor)
        {
            major++;
            
            var minMinor = endpoints.Where(x => x.Major == major).Min(x => x.Minor);
            var maxMinor = endpoints.Where(x => x.Major == major).Max(x => x.Minor);
            
            lookup.Add(minMajor, (
                minMinor,
                maxMinor
            ));
        }

        major = endpoints.Min(x => x.Major);

        var collection = new List<AppEndpoint>();
        endpoints.ForEach(x =>
        {
            major = endpoints.Min(x => x.Major);

            while (major <= maxMajor)
            {
                var maxMinor = lookup[major].max;

                var minor = lookup[major].min;
                
                while (minor <= maxMinor)
                {
                    collection.Add(new AppEndpoint(major, minor, x.Path, x.Method, x.EndpointAction, x.MapAuth));
                    minor++;
                }

                major++;
            }
        });

        var endpointsToMap = collection
            .DistinctBy(x => new { x.Major, x.Minor, x.Path, x.Method })
            .ToArray();

        foreach(var e in endpointsToMap)
        {
            var fullPath = $"api/v{e.Major}.{e.Minor}/{e.Path}";
            var endpoint = e.Method switch
            {
                HttpMethod.Get => app.MapGet(fullPath, e.EndpointAction),
                HttpMethod.Post => app.MapPost(fullPath, e.EndpointAction),
                _ => throw new InvalidOperationException($"HttpMethod {e.Method} not supported")
            };

            e.MapAuth?.Invoke(endpoint);
        }
    }
}
