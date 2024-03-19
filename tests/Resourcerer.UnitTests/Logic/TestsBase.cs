using FakeItEasy;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Resourcerer.Api;
using Resourcerer.Api.Endpoints;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Utilities;

using QU = Resourcerer.DataAccess.Utilities.Query.Instances;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly AppDbContext _testDbContext;

    public TestsBase()
    {
        _testDbContext = new ContextCreator().GetTestDbContext();
    }

    [Fact]
    public async void Scratchpad()
    {
        var app = Webapi.Build(Array.Empty<string>());
        EndpointMapper.Map(app);
        
        var runTask = app.RunAsync();

        var endpoints = app.Services
            .GetServices<EndpointDataSource>()
            .SelectMany(es => es.Endpoints)
            .ToArray();

        foreach (var endpoint in endpoints)
        {
            if (endpoint is RouteEndpoint routeEndpoint)
            {
                _ = routeEndpoint.RoutePattern.RawText;
                _ = routeEndpoint.RoutePattern.PathSegments;
                _ = routeEndpoint.RoutePattern.Parameters;
                _ = routeEndpoint.RoutePattern.InboundPrecedence;
                _ = routeEndpoint.RoutePattern.OutboundPrecedence;
            }

            var routeNameMetadata = endpoint.Metadata.OfType<Microsoft.AspNetCore.Routing.RouteNameMetadata>().FirstOrDefault();
            _ = routeNameMetadata?.RouteName;

            var httpMethodsMetadata = endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault();
            _ = httpMethodsMetadata?.HttpMethods; // [GET, POST, ...]

            // There are many more metadata types available...
        }

        app.StopAsync().Wait();
    }

    [Fact]
    public void QueryInspection()
    {
        var order = new ItemProductionOrder();

        var query = _testDbContext.Instances
            .Where(x => order.InstancesUsedIds.Contains(x.Id))
            .Select(QU.Expand(x => new Instance
            {
                ReservedEvents = x.ReservedEvents
            }))
            .ToQueryString();

        var _ = 0;
    }

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();
}
