using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.V1_0.Commands.Items;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1_0.Commands.Items.Events.Production;

public class FinishItemProductionOrderTests : TestsBase
{
    private readonly FinishItemProductionOrder.Handler _sut;
    public FinishItemProductionOrderTests()
    {
        _sut = new(_testDbContext);
    }

    [Fact]
    public void HappyPath()
    {
        // arrange
        var fd = Faking.FakeData(_testDbContext, 2, 2);
        var order = Faking.FakeOrder(_testDbContext, fd, x =>
        {
            x.Quantity = 2;
            x.StartedEvent = JsonEntityBase.CreateEntity(() => new ItemProductionStartedEvent());
        });
        var dto = new FinishItemProductionOrderRequest
        {
            ProductionOrderId = order.Id
        };

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        AssertPersistedData(order.Id);
    }

    private void AssertPersistedData(Guid itemProductionOrderId)
    {
        var order = _testDbContext.ItemProductionOrders
            .First(x => x.Id == itemProductionOrderId);

        Assert.NotNull(order.FinishedEvent);
        
        var newInstance = _testDbContext.Instances
            .First(x =>
                x.ItemId == order.ItemId &&
                x.Quantity == order.Quantity);
    }
}
