using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items.Events.Production;

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
        var fd = Faking.FakeData(_forger, 2, 2);
        var order = Faking.FakeOrder(_forger, fd, x => x.Quantity = 2);
        fd.Elements.ForEach(el =>
        {
            el.Instances.ForEach(i =>
            {
                _forger.Fake<InstanceReservedEvent>(x =>
                {
                    x.ItemProductionOrderId = order.Id;
                    x.Quantity = 1;
                    x.Instance = i;
                });
            });
        });

        var dto = new V1StartItemProductionOrderCommand
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
        var dto = new V1StartItemProductionOrderCommand
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
        var fd = Faking.FakeData(_forger, 2, 2);
        var order = Faking.FakeOrder(_forger, fd, x =>
        {
            x.Quantity = 2;
            x.CancelledEvent = AppDbJsonField.Create(() => new ItemProductionOrderCancelledEvent());
        });
        var dto = new V1StartItemProductionOrderCommand
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
        var fd = Faking.FakeData(_forger, 2, 2);
        var order = Faking.FakeOrder(_forger, fd, x =>
        {
            x.Quantity = 2;
            x.FinishedEvent = AppDbJsonField.Create(() => new ItemProductionFinishedEvent());
        });
        var dto = new V1StartItemProductionOrderCommand
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
        var fd = Faking.FakeData(_forger, 2, 2);
        var order = Faking.FakeOrder(_forger, fd, x =>
        {
            x.Quantity = 2;
            x.StartedEvent = AppDbJsonField.Create(() => new ItemProductionStartedEvent());
        });
        var dto = new V1StartItemProductionOrderCommand
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