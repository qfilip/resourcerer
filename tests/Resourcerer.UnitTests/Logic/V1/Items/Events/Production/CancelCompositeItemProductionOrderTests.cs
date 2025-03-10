﻿using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items.Events.Production;

public class CancelCompositeItemProductionOrderTests : TestsBase
{
    private readonly CancelCompositeItemProductionOrder.Handler _sut;
    public CancelCompositeItemProductionOrderTests()
    {
        _sut = new CancelCompositeItemProductionOrder.Handler(_ctx);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var order = FakeData();
        var dto = new V1CancelCompositeItemProductionOrderCommand
        {
            ProductionOrderId = order.Id,
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
                    .Where(x => x.Id == dto.ProductionOrderId)
                    .First();

                Assert.NotNull(data.CancelledEvent);

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
        var dto = new V1CancelCompositeItemProductionOrderCommand
        {
            ProductionOrderId = Guid.NewGuid(),
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
                Assert.Null(data.CancelledEvent);
            }
        );
    }

    [Fact]
    public void DataCorrupted__Exception()
    {
        // arrange
        var order = FakeData(x => x.InstancesUsedIds = []);
        var dto = new V1CancelCompositeItemProductionOrderCommand
        {
            ProductionOrderId = order.Id,
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
            x.StartedEvent = AppDbJsonField.CreateKeyless(() => new ItemProductionStartedEvent());
        });
        var dto = new V1CancelCompositeItemProductionOrderCommand
        {
            ProductionOrderId = order.Id,
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
                Assert.Null(data.CancelledEvent);
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
        var dto = new V1CancelCompositeItemProductionOrderCommand
        {
            ProductionOrderId = order.Id,
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

        var composite = _forger.Fake<Item>();

        var items = new List<Item>()
        {
            _forger.Fake<Item>(),
            _forger.Fake<Item>()
        };

        _forger.Fake<Recipe>(x =>
        {
            x.CompositeItem = composite;
            x.RecipeExcerpts = items.Select(i =>
                _forger.Fake<RecipeExcerpt>(re =>
                {
                    re.Element = i;
                    re.Quantity = 1;
                    re.Recipe = x;
                })

            ).ToList();
        });

        var instances = items
            .Select(x =>
            {
                var instance = _forger.Fake<Instance>(i => i.Item = x);
                _forger.Fake<InstanceReservedEvent>(x =>
                {
                    x.ItemProductionOrderId = productionOrderId;
                    x.InstanceId = instance.Id;
                    x.Instance = instance;
                });

                return instance;
            })
            .ToArray();

        var order = _forger.Fake<ItemProductionOrder>(x =>
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
