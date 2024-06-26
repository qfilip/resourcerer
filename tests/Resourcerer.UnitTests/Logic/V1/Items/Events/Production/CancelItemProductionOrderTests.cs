﻿using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items.Events.Production;

public class CancelItemProductionOrderTests : TestsBase
{
    private readonly CancelItemProductionOrder.Handler _sut;
    public CancelItemProductionOrderTests()
    {
        _sut = new CancelItemProductionOrder.Handler(_ctx);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var order = FakeData();
        var dto = new V1CancelItemProductionOrderRequest
        {
            ProductionOrderEventId = order.Id,
            Reason = "Test"
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var data = _ctx.ItemProductionOrders
                    .Where(x => x.Id == dto.ProductionOrderEventId)
                    .First();

                Assert.NotNull(data.CanceledEvent);

                var reservedEvents = _ctx.InstanceReservedEvents
                    .Where(x => order.InstancesUsedIds.Contains(x.InstanceId))
                    .ToArray();

                Assert.True(reservedEvents.All(x => x.CancelledEvent != null));
            }
        );
    }

    [Fact]
    public void NotFound__NotFound()
    {
        // arrange
        var _ = FakeData();
        var dto = new V1CancelItemProductionOrderRequest
        {
            ProductionOrderEventId = Guid.NewGuid(),
            Reason = "Test"
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.NotFound, result.Status),
            () =>
            {
                _ctx.Clear();
                var data = _ctx.ItemProductionOrders.First();
                Assert.Null(data.CanceledEvent);
            }
        );
    }

    [Fact]
    public void DataCorrupted__Exception()
    {
        // arrange
        var order = FakeData(x => x.InstancesUsedIds = []);
        var dto = new V1CancelItemProductionOrderRequest
        {
            ProductionOrderEventId = order.Id,
            Reason = "Test"
        };

        _ctx.SaveChanges();

        // act
        var action = async () =>
        {
            await _sut.Handle(dto);
            return Task.CompletedTask;
        };

        // assert
        Assert.ThrowsAsync<DataCorruptionException>(action);
    }

    [Fact]
    public void StartedEventExists__Rejected()
    {
        // arrange
        var order = FakeData(x =>
        {
            x.StartedEvent = AppDbJsonField.Create(() => new ItemProductionStartedEvent());
        });
        var dto = new V1CancelItemProductionOrderRequest
        {
            ProductionOrderEventId = order.Id,
            Reason = "Test"
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Rejected, result.Status),
            () =>
            {
                _ctx.Clear();
                var data = _ctx.ItemProductionOrders.First();
                Assert.Null(data.CanceledEvent);
            }
        );
    }

    [Fact]
    public void InvalidDataStored__Exception()
    {
        // arrange
        var order = FakeData(x =>
        {
            x.InstancesUsedIds = [Guid.NewGuid(), Guid.NewGuid()];
        });
        var dto = new V1CancelItemProductionOrderRequest
        {
            ProductionOrderEventId = order.Id,
            Reason = "Test"
        };

        _ctx.SaveChanges();

        // act
        var action = async () =>
        {
            await _sut.Handle(dto);
            return Task.CompletedTask;
        };

        // assert
        Assert.ThrowsAsync<DataCorruptionException>(action);
    }

    private ItemProductionOrder FakeData(Action<ItemProductionOrder>? modifier = null)
    {
        var productionOrderId = Guid.NewGuid();

        var composite = DF.Fake<Item>(_ctx);

        var items = new List<Item>()
        {
            DF.Fake<Item>(_ctx),
            DF.Fake<Item>(_ctx)
        };

        items.ForEach(i => DF.Fake<Excerpt>(_ctx, x =>
        {
            x.Composite = composite;
            x.Element = i;
            x.Quantity = 1;
        }));

        var instances = items
            .Select(x =>
            {
                var instance = DF.Fake<Instance>(_ctx, i => i.Item = x);
                DF.Fake<InstanceReservedEvent>(_ctx, x =>
                {
                    x.ItemProductionOrderId = productionOrderId;
                    x.InstanceId = instance.Id;
                    x.Instance = instance;
                });

                return instance;
            })
            .ToArray();

        var order = DF.Fake<ItemProductionOrder>(_ctx, x =>
        {
            x.Id = productionOrderId;
            x.ItemId = composite.Id;
            x.InstancesUsedIds = instances.Select(x => x.Id).ToArray();
            x.Quantity = 1;
            x.Reason = "test";

            modifier?.Invoke(x);
        });

        return order;
    }
}
