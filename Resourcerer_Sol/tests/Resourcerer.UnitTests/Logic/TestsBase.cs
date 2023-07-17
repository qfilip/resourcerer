using FakeItEasy;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Logic.Queries.Items;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly AppDbContext _testDbContext;
    public TestsBase()
    {
        _testDbContext = new ContextCreator().GetTestDbContext();
    }

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();

    [Fact]
    public void Test()
    {
        var ctx = new ContextCreator().GetTestDbContext();
        Mocker.MockDbData(ctx);

        var sand = ctx.Items.Where(x => x.Name == "sand").First();
        var now = Mocker.Now.AddMonths(4);
        
        Mocker.MockDeliveredEvent(ctx, x => x.CreatedAt = Mocker.Now.AddMonths(1), sand);
        Mocker.MockDeliveredEvent(ctx, x => x.CreatedAt = Mocker.Now.AddMonths(2), sand);
        Mocker.MockDeliveredEvent(ctx, x => x.CreatedAt = Mocker.Now.AddMonths(3), sand);

        ctx.SaveChanges();

        var handler = new GetItemStatistics.Handler(ctx);
        handler.Handle((sand.Id, now)).GetAwaiter().GetResult();
    }
}
