using Resourcerer.Logic.Queries.Items;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.Queries;

public class GetItemStatisticsTests : TestsBase
{
    private readonly GetItemStatistics.Handler _handler;
    public GetItemStatisticsTests()
    {
        Mocker.MockDbData(_testDbContext);
        _handler = new(_testDbContext);
    }
}
