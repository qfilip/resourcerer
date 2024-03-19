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
        var order = Faking.FakeOrder(_testDbContext, fd, x => x.Quantity = 2);
        var dto = new FinishItemProductionOrderRequest
        {
            ProductionOrderId = order.Id
        };

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        // AssertCorrectEventsCreated(dto);
    }
}
