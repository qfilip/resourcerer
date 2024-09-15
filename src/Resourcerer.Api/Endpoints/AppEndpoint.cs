using Resourcerer.Api.Services.StaticServices;
using System.Text.Json;
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

        var major = minMajor;
        while (major <= maxMajor)
        {   
            var minMinor = endpoints.Where(x => x.Major == major).Min(x => x.Minor);
            var maxMinor = endpoints.Where(x => x.Major == major).Max(x => x.Minor);
            
            lookup.Add(major, (minMinor,maxMinor));

            major++;
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

        var apiVersions = app.NewApiVersionSet();

        foreach(var key in lookup.Keys)
        {
            var min = lookup[key].min;
            var max = lookup[key].max;
            var current = min;
            
            while(current <= max)
            {
                apiVersions.HasApiVersion(new Asp.Versioning.ApiVersion(key, current));
                current++;
            }
        }

        var apiVersionSet = apiVersions.ReportApiVersions().Build();

        foreach (var e in endpointsToMap)
        {
            // var fullPath = $"api/v{e.Major}.{e.Minor}/{e.Path}";
            var fullPath = $"v{e.Major}.{e.Minor}/{e.Path}";
            var endpoint = e.Method switch
            {
                HttpMethod.Get => app.MapGet(fullPath, e.EndpointAction),
                HttpMethod.Put => app.MapPut(fullPath, e.EndpointAction),
                HttpMethod.Patch => app.MapPatch(fullPath, e.EndpointAction),
                HttpMethod.Post => app.MapPost(fullPath, e.EndpointAction),
                HttpMethod.Delete => app.MapDelete(fullPath, e.EndpointAction),
                _ => throw new InvalidOperationException($"HttpMethod {e.Method} not supported")
            };

            // adding this results with returning 404, but it is required
            //endpoint
            //    .WithApiVersionSet(apiVersionSet)
            //    .MapToApiVersion(e.Major, e.Minor);

            if (AppStaticData.Auth.Enabled)
                e.MapAuth?.Invoke(endpoint);
        }
    }
}
