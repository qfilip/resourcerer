﻿using Resourcerer.Api.Services.StaticServices;
using Resourcerer.Dtos;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

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

        MapAllVersions(serviceTypes, app);
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

    private static void MapAllVersions(List<AppEndpoint> endpoints, WebApplication app)
    {
        // check name duplicates
        endpoints.ForEach(e =>
        {
            var count = endpoints.Where(x =>
                x.Path == e.Path &&
                x.Major == e.Major &&
                x.Minor == e.Minor &&
                x.Method == e.Method
            ).Count();

            if(count > 1)
            {
                var message = $"More than one endpoint found with path {e.Path} and method {e.Method}";
                throw new InvalidOperationException(message);
            }
        });

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

        foreach (var key in lookup.Keys)
        {
            var min = lookup[key].min;
            var max = lookup[key].max;
            var current = min;

            while (current <= max)
            {
                apiVersions.HasApiVersion(new Asp.Versioning.ApiVersion(key, current));
                current++;
            }
        }

        var apiVersionSet = apiVersions.ReportApiVersions().Build();

        string MapPath(int major, int minor, string path) => $"v{major}.{minor}/{path.Split('/')[0]}";

        var groups = endpointsToMap
            .Select(x => MapPath(x.Major, x.Minor, x.Path))
            .Distinct()
            .Select(x => new { Prefix = x, Builder = app.MapGroup(x) })
            .ToArray();

        foreach (var e in endpointsToMap)
        {
            var endpointPathParts = e.Path
                .Split('/')
                .Skip(1);

            var endpointPath = string.Join("/", endpointPathParts);

            var group = groups
                .Where(x => x.Prefix == MapPath(e.Major, e.Minor, e.Path))
                .Single();

            var fullPath = $"{group.Prefix}/{endpointPath}";
            var endpoint = e.Method switch
            {
                HttpMethod.Get => group.Builder.MapGet(endpointPath, e.EndpointAction),
                HttpMethod.Put => group.Builder.MapPut(endpointPath, e.EndpointAction),
                HttpMethod.Patch => group.Builder.MapPatch(endpointPath, e.EndpointAction),
                HttpMethod.Post => group.Builder.MapPost(endpointPath, e.EndpointAction),
                HttpMethod.Delete => group.Builder.MapDelete(endpointPath, e.EndpointAction),
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
