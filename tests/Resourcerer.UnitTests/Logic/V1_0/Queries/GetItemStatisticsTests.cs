using Resourcerer.Logic.Queries.V1;

namespace Resourcerer.UnitTests.Logic.V1.Queries.Categories;

public class GetItemStatisticsTests : TestsBase
{
    private readonly GetItemStatistics.Handler _handler;
    public GetItemStatisticsTests()
    {
        _handler = new(_ctx);
    }

    [Fact]
    public void Do()
    {

    }

}
