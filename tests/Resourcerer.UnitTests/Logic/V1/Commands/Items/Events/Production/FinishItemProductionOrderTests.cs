using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands.Items;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Commands.Items;

public class FinishItemProductionOrderTests : TestsBase
{
    private readonly FinishItemProductionOrder.Handler _sut;
    public FinishItemProductionOrderTests()
    {
        _sut = new(_ctx);
    }

    [Fact]
    public void HappyPath()
    {
        // arrange
        var fd = Faking.FakeData(_ctx, 2, 2);
        var order = Faking.FakeOrder(_ctx, fd, x =>
        {
            x.Quantity = 2;
            x.StartedEvent = AppDbJsonField.Create(() => new ItemProductionStartedEvent());
        });
        var dto = new V1FinishItemProductionOrderRequest
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
                AssertDataIntegrity(order.Id);
            }
        );
    }

    [Fact]
    public void Order_NotFound__NotFound()
    {
        // arrange
        var dto = new V1FinishItemProductionOrderRequest
        {
            ProductionOrderId = Guid.NewGuid()
        };

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void OrderCancelled__Rejected()
    {
        // arrange
        var fd = Faking.FakeData(_ctx, 2, 2);
        var order = Faking.FakeOrder(_ctx, fd, x =>
        {
            x.CanceledEvent = AppDbJsonField.Create(() => new ItemProductionOrderCancelledEvent());
        });
        var dto = new V1FinishItemProductionOrderRequest
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
    public void OrderNotStarted__Rejected()
    {
        // arrange
        var fd = Faking.FakeData(_ctx, 2, 2);
        var order = Faking.FakeOrder(_ctx, fd);
        var dto = new V1FinishItemProductionOrderRequest
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
    public void Idempotency_OrderFinished__Ok()
    {
        // arrange
        var fd = Faking.FakeData(_ctx, 2, 2);
        var order = Faking.FakeOrder(_ctx, fd, x =>
        {
            x.StartedEvent = AppDbJsonField.Create(() => new ItemProductionStartedEvent());
            x.FinishedEvent = AppDbJsonField.Create(() => new ItemProductionFinishedEvent());
        });
        var dto = new V1FinishItemProductionOrderRequest
        {
            ProductionOrderId = order.Id
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    private void AssertDataIntegrity(Guid itemProductionOrderId)
    {
        var order = _ctx.ItemProductionOrders
            .First(x => x.Id == itemProductionOrderId);

        Assert.NotNull(order.FinishedEvent);
        
        var newInstance = _ctx.Instances
            .First(x =>
                x.ItemId == order.ItemId &&
                x.Quantity == order.Quantity);
    }
}
