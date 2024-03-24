using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands.Items;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1.Commands.Items;

public class StartItemProductionOrderTests : TestsBase
{
    private readonly StartItemProductionOrder.Handler _sut;
    public StartItemProductionOrderTests()
    {
        _sut = new(_ctx);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var fd = Faking.FakeData(_ctx, 2, 2);
        var order = Faking.FakeOrder(_ctx, fd, x => x.Quantity = 2);
        fd.Elements.ForEach(el =>
        {
            el.Instances.ForEach(i =>
            {
                DF.Fake<InstanceReservedEvent>(_ctx, x =>
                {
                    x.ItemProductionOrderId = order.Id;
                    x.Quantity = 1;
                    x.Instance = i;
                });
            });
        });
        
        var dto = new V1StartItemProductionOrderRequest
        {
            ProductionOrderId = order.Id
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
                var dbOrder = _ctx.ItemProductionOrders.First(x => x.Id == order.Id);
                Assert.NotNull(dbOrder.StartedEvent);

                var reservedInstances = _ctx.Instances
                    .Where(x => dbOrder.InstancesUsedIds.Contains(x.Id))
                    .Include(x => x.ReservedEvents)
                    .ToArray();

                Assert.True(reservedInstances.Any());
                Assert.True(reservedInstances.All(x =>
                {
                    return x.ReservedEvents
                        .Where(x =>
                            x.ItemProductionOrderId == order.Id &&
                            x.Quantity == 1)
                        .Count() == 1;
                }));
            }
        );
    }

    [Fact]
    public void Order_NotFound__NotFound()
    {
        var dto = new V1StartItemProductionOrderRequest
        {
            ProductionOrderId = Guid.NewGuid()
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void OrderCancelled__Rejected()
    {
        var fd = Faking.FakeData(_ctx, 2, 2);
        var order = Faking.FakeOrder(_ctx, fd, x =>
        {
            x.Quantity = 2;
            x.CanceledEvent = AppDbJsonField.Create(() => new ItemProductionOrderCancelledEvent());
        });
        var dto = new V1StartItemProductionOrderRequest
        {
            ProductionOrderId = order.Id
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void OrderFinished__Rejected()
    {
        var fd = Faking.FakeData(_ctx, 2, 2);
        var order = Faking.FakeOrder(_ctx, fd, x =>
        {
            x.Quantity = 2;
            x.FinishedEvent = AppDbJsonField.Create(() => new ItemProductionFinishedEvent());
        });
        var dto = new V1StartItemProductionOrderRequest
        {
            ProductionOrderId = order.Id
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void Idempotent_OrderStarted__Ok()
    {
        var fd = Faking.FakeData(_ctx, 2, 2);
        var order = Faking.FakeOrder(_ctx, fd, x =>
        {
            x.Quantity = 2;
            x.StartedEvent = AppDbJsonField.Create(() => new ItemProductionStartedEvent());
        });
        var dto = new V1StartItemProductionOrderRequest
        {
            ProductionOrderId = order.Id
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}