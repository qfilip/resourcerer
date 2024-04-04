﻿using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;
using System.Text.Json;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly TestDbContext _ctx;

    public TestsBase()
    {
        _ctx = new ContextCreator().GetTestDbContext();
    }

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();

    [Fact]
    public void TryMockDb()
    {
        DF.FakeDatabase(_ctx);
        var path = Path.Combine(Directory.GetCurrentDirectory(), "data.json");

        var data = new
        {
            Compaines = _ctx.Companies.AsNoTracking().ToArray(),
            AppUsers = _ctx.AppUsers.AsNoTracking().ToArray(),
            Categories = _ctx.Categories.AsNoTracking().ToArray(),
            UnitsOfMeasure = _ctx.UnitsOfMeasure.AsNoTracking().ToArray(),
            Excerpts = _ctx.Excerpts.AsNoTracking().ToArray(),
            Prices = _ctx.Prices.AsNoTracking().ToArray(),
            Items = _ctx.Items.AsNoTracking().ToArray(),
            ItemProductionOrders = _ctx.ItemProductionOrders.AsNoTracking().ToArray(),
            Instances = _ctx.Instances.AsNoTracking().ToArray(),
            InstanceOrderedEvents = _ctx.InstanceOrderedEvents.AsNoTracking().ToArray(),
            InstanceReservedEvents = _ctx.InstanceReservedEvents.AsNoTracking().ToArray(),
            InstanceDiscardedEvents = _ctx.InstanceDiscardedEvents.AsNoTracking().ToArray()
        };
        
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
        
        File.WriteAllText(path, json);
    }
}
