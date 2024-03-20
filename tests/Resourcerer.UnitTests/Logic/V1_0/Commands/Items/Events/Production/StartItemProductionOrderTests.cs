using Resourcerer.Logic.V1.Commands.Items;

namespace Resourcerer.UnitTests.Logic.V1.Commands.Items;

public class StartItemProductionOrderTests : TestsBase
{
    private readonly StartItemProductionOrder.Handler _sut;
    public StartItemProductionOrderTests()
    {
        _sut = new(_ctx);
    }

    [Fact]
    public void Todo()
    {
        Assert.True(false);
    }
}