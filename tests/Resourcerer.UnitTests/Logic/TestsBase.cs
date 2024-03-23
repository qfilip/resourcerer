using FakeItEasy;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly TestDbContext _ctx;

    public TestsBase()
    {
        _ctx = new ContextCreator().GetTestDbContext();
    }

    [Fact]
    public void Scratchpad()
    {
        var instance = DF.Fake<Instance>(_ctx);
        _ctx.SaveChanges();

        _ = 0;
    }

    [Fact(Skip = "demonstration")]
    public void FakingData()
    {
        var company = DF.Fake<Company>(_ctx, x => x.Name = "acme inc");
        var instance = DF.Fake<Instance>(_ctx, x => x.OwnerCompany = company);

        _ctx.SaveChanges();

        var instanceCompanyName = _ctx.Instances
            .Select(x => new { x.Id, x.OwnerCompany!.Name })
            .First(x => x.Id == instance.Id)
            .Name;
    }

    //[Fact]
    //public async void Scratchpad()
    //{
    //    var app = Webapi.Build(Array.Empty<string>());
    //    EndpointMapper.Map(app);

    //    var runTask = app.RunAsync();

    //    var endpoints = app.Services
    //        .GetServices<EndpointDataSource>()
    //        .SelectMany(es => es.Endpoints)
    //        .ToArray();

    //    foreach (var endpoint in endpoints)
    //    {
    //        if (endpoint is RouteEndpoint routeEndpoint)
    //        {
    //            _ = routeEndpoint.RoutePattern.RawText;
    //            _ = routeEndpoint.RoutePattern.PathSegments;
    //            _ = routeEndpoint.RoutePattern.Parameters;
    //            _ = routeEndpoint.RoutePattern.InboundPrecedence;
    //            _ = routeEndpoint.RoutePattern.OutboundPrecedence;
    //        }

    //        var routeNameMetadata = endpoint.Metadata.OfType<Microsoft.AspNetCore.Routing.RouteNameMetadata>().FirstOrDefault();
    //        _ = routeNameMetadata?.RouteName;

    //        var httpMethodsMetadata = endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault();
    //        _ = httpMethodsMetadata?.HttpMethods; // [GET, POST, ...]

    //        // There are many more metadata types available...
    //    }

    //    app.StopAsync().Wait();
    //}

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();
}
