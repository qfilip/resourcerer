using Resourcerer.Logic.V1_0.Commands.Items;

namespace Resourcerer.UnitTests.Logic.V1_0.Commands.Items;

public class StartItemProductionOrderTests : TestsBase
{
    private readonly StartItemProductionOrder.Handler _sut;
    public StartItemProductionOrderTests()
    {
        _sut = new(_testDbContext);
    }

    [Fact]
    public void Todo()
    {
        Assert.True(false);
    }
}