using Resourcerer.Logic.Queries.Items;

namespace Resourcerer.UnitTests.Logic.Queries;

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
