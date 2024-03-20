using Resourcerer.Logic.Queries.V1;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class GetItemStatisticsTests : TestsBase
{
    private readonly GetItemStatistics.Handler _handler;
    public GetItemStatisticsTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void Do()
    {

    }

}
