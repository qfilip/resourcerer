using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.Exceptions;
using Resourcerer.Logic.V1.Commands.Items;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1.Commands.Items.Events.Production;

public class CancelItemProductionOrderTests : TestsBase
{
    private readonly CancelItemProductionOrder.Handler _sut;
    public CancelItemProductionOrderTests()
    {
        _sut = new CancelItemProductionOrder.Handler(_testDbContext);
    }

    [Fact]
    public void HappyPath()
    {
        // arrange
        var order = FakeData();
        var dto = new V1CancelItemProductionOrderRequest
        {
            ProductionOrderEventId = order.Id,
            Reason = "Test"
        };

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        
        var data = _testDbContext.ItemProductionOrders.First();
        Assert.NotNull(data.CanceledEvent);
        
        var instanceData = _testDbContext.Instances
            .Where(x => order.InstancesUsedIds.Contains(x.Id))
            .ToArray();

        Assert.True(instanceData.All(x => x.ReservedEvents[0].CancelledEvent != null));
    }

    [Fact]
    public void When_NotFound_Then_NotFound()
    {
        // arrange
        var order = FakeData();
        var dto = new V1CancelItemProductionOrderRequest
        {
            ProductionOrderEventId = Guid.NewGuid(),
            Reason = "Test"
        };

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
        var data = _testDbContext.ItemProductionOrders.First();
        Assert.Null(data.CanceledEvent);
    }

    [Fact]
    public void When_DataCorrupted_Then_Exception()
    {
        // arrange
        var order = FakeData(x => x.InstancesUsedIds = []);
        var dto = new V1CancelItemProductionOrderRequest
        {
            ProductionOrderEventId = order.Id,
            Reason = "Test"
        };

        _testDbContext.SaveChanges();

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
    public void When_StartedEventExists_Then_Rejected()
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

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
        var data = _testDbContext.ItemProductionOrders.First();
        Assert.Null(data.CanceledEvent);
    }

    [Fact]
    public void When_InvalidDataStored_Then_Exception()
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

        _testDbContext.SaveChanges();

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

        var composite = DF.FakeItem(_testDbContext);
        
        var elementOne = DF.FakeItem(_testDbContext);
        var elementTwo = DF.FakeItem(_testDbContext);

        DF.FakeExcerpts(_testDbContext, composite, [
            (elementOne, 1),
            (elementTwo, 1),
        ]);

        var instances = Enumerable.Range(0, 2)
            .Select(x =>
            {
                var id = x % 2 == 0 ? elementOne.Id : elementTwo.Id;
                return DF.FakeInstance(_testDbContext, i =>
                {
                    i.ItemId = id;
                    i.ReservedEvents = new()
                    {
                        AppDbJsonField.Create(() => new InstanceReservedEvent
                        {
                            ItemProductionOrderId = productionOrderId,
                        })
                    };
                });
            })
            .ToArray();

        var order = DF.FakeItemProductionOrder(_testDbContext, x =>
        {
            x.Id = productionOrderId;
            x.ItemId = composite.Id;
            x.InstancesUsedIds = instances.Select(x => x.Id).ToArray();
            x.Quantity = 1;
            x.Reason = "test";
        });

        modifier?.Invoke(order);

        return order;
    }
}
