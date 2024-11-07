using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items.Events.Production;

public class CancelElementItemProductionOrderTests : TestsBase
{
    private readonly CancelElementItemProductionOrder.Handler _sut;
    public CancelElementItemProductionOrderTests()
    {
        _sut = new CancelElementItemProductionOrder.Handler(_ctx);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var orderId = _forger.Fake<Item>(x =>
        {
            x.ProductionOrders =
            [
                _forger.Fake<ItemProductionOrder>()
            ];
        }).ProductionOrders.First().Id;

        var command = new V1CancelElementItemProductionOrderCommand
        {
            ProductionOrderId = orderId,
            Reason = "because..."
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(command).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                var order = _ctx.ItemProductionOrders.First();
                Assert.NotNull(order.CancelledEvent);
                Assert.Equal(command.Reason, order.CancelledEvent!.Reason);
            });
    }

    [Fact]
    public void OrderNotFound__NotFound()
    {
        // arrange
        var item = _forger.Fake<Item>(x =>
        {
            x.ProductionOrders =
            [
                _forger.Fake<ItemProductionOrder>()
            ];
        });

        var command = new V1CancelElementItemProductionOrderCommand
        {
            ProductionOrderId = Guid.NewGuid(),
            Reason = "because..."
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(command).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void OrderDoesNotExist__NotFound()
    {
        // arrange
        var _ = _forger.Fake<Item>();

        var command = new V1CancelElementItemProductionOrderCommand
        {
            ProductionOrderId = Guid.NewGuid(),
            Reason = "because..."
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(command).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void ProductionStarted__Rejected()
    {
        // arrange
        var orderId = _forger.Fake<Item>(x =>
        {
            x.ProductionOrders =
            [
                _forger.Fake<ItemProductionOrder>(o =>
                {
                    o.StartedEvent = AppDbJsonField.CreateKeyless(() => new ItemProductionStartedEvent());
                })
            ];
        }).ProductionOrders.First().Id;

        var command = new V1CancelElementItemProductionOrderCommand
        {
            ProductionOrderId = orderId,
            Reason = "because..."
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(command).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void ProductionFinished__Rejected()
    {
        // arrange
        var orderId = _forger.Fake<Item>(x =>
        {
            x.ProductionOrders =
            [
                _forger.Fake<ItemProductionOrder>(o =>
                {
                    o.FinishedEvent = AppDbJsonField.CreateKeyless(() => new ItemProductionFinishedEvent());
                })
            ];
        }).ProductionOrders.First().Id;

        var command = new V1CancelElementItemProductionOrderCommand
        {
            ProductionOrderId = orderId,
            Reason = "because..."
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(command).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}