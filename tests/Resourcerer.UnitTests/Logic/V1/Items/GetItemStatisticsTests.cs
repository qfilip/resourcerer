using Resourcerer.Logic.V1.Items;

namespace Resourcerer.UnitTests.Logic.V1.Items;

public class GetItemStatisticsTests : TestsBase
{
    private readonly GetItemStatistics.Handler _handler;
    public GetItemStatisticsTests()
    {
        _handler = new(_ctx, new());
    }

    [Fact]
    public void Do()
    {

    }

}
