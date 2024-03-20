﻿using FakeItEasy;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Resourcerer.Api;
using Resourcerer.Api.Endpoints;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;
using System.Linq.Expressions;
using QU = Resourcerer.DataAccess.Utilities.Query.Instances;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly TestDbContext _ctx;

    public TestsBase()
    {
        _ctx = new ContextCreator().GetTestDbContext();
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

    //[Fact]
    //public void QueryInspection()
    //{
    //    var order = new ItemProductionOrder();

    //    var exp1 = QU.Expand(x => new Instance
    //    {
    //        Id = x.Id,
    //        ReservedEventsJson = x.ReservedEventsJson
    //    });
    //    Expression<Func<Instance, Instance>> exp2 = x => new Instance { Id = x.Id, };

    //    var s1 = exp1.ToString();
    //    var s2 = exp2.ToString();

    //    var q1 = _testDbContext.Instances
    //        .Where(x => order.InstancesUsedIds.Contains(x.Id))
    //        .Select(exp1)
    //        .ToQueryString();

    //    var q2 = _testDbContext.Instances
    //        .Where(x => order.InstancesUsedIds.Contains(x.Id))
    //        .Select(QU.Expand(x => new Instance
    //        {
    //            Id = x.Id,
    //            ReservedEvents = x.ReservedEvents
    //        }))
    //        .ToQueryString();

    //    var q3 = _testDbContext.Instances
    //        .Where(x => order.InstancesUsedIds.Contains(x.Id))
    //        .Select(exp2)
    //        .ToQueryString();

    //    var _ = 0;
    //}

    //[Fact]
    //public void ChangeTracking()
    //{
    //    // https://learn.microsoft.com/en-us/ef/core/querying/tracking
    //    DF.FakeInstance(_testDbContext);
    //    _testDbContext.SaveChanges();

    //    var i = _testDbContext.Instances
    //        .Select(QU.Expand(x => new Instance
    //        {
    //            ReservedEventsJson = x.ReservedEventsJson
    //        }))
    //        .First();

    //    _testDbContext.Attach(i);
    //    i.Quantity = 7;
    //    var arr = _testDbContext.ChangeTracker.Entries().ToArray();
    //    _testDbContext.SaveChanges();

    //    var v = _testDbContext.Instances.First();

    //    _ = 0;
    //}

    //[Fact]
    //public void ChangeTrackingV2()
    //{
    //    // https://learn.microsoft.com/en-us/ef/core/querying/tracking
    //    var o = DF.FakeItemProductionOrder(_testDbContext, x =>
    //    {
    //        x.Quantity = 1;
    //        x.StartedEvent = JsonEntityBase.CreateEntity(() => new ItemProductionStartedEvent());
    //    });
    //    _testDbContext.SaveChanges();

    //    var o2 = _testDbContext.ItemProductionOrders
    //        .Select(x => new ItemProductionOrder
    //        {
    //            Id = x.Id,
    //            ItemId = x.ItemId,
    //            Item = new Item { CategoryId = x.Item!.CategoryId },
    //            StartedEventJson = x.StartedEventJson,
    //            FinishedEventJson = x.FinishedEventJson
    //        })
    //        .AsNoTracking()
    //        .First(x => x.Id == o.Id);

    //    o2.FinishedEvent = JsonEntityBase.CreateEntity(() => new ItemProductionFinishedEvent());

    //    _testDbContext.ItemProductionOrders.Attach(o2);
        
    //    _testDbContext.Entry(o2).Property(x => x.FinishedEventJson).IsModified = true;
    //    var r = _testDbContext.ItemProductionOrders.Entry(o2).Property(x => x.ItemId).IsModified; // bugged
    //    var r2 = _testDbContext.Entry(o2).Property(x => x.CanceledEventJson).IsModified;
    //    _testDbContext.SaveChanges();

    //    var o3 = _testDbContext.ItemProductionOrders
    //        .AsNoTracking()
    //        .First(x => x.Id == o.Id && x.Quantity == 1);

    //    _ = 0;
    //}

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();
}
