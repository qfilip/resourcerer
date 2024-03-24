﻿using Resourcerer.DataAccess.Entities;
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
        _ctx.ChangeTracker.Clear();
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        AssertPersistedData(order.Id);
    }

    private void AssertPersistedData(Guid itemProductionOrderId)
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
