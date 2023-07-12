using FakeItEasy;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
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
        ctx.SaveChanges();
        var id = ctx.Items.Where(x => x.Name == "mortar").First().Id;
        ctx.ChangeTracker.Clear();

        var handler = new GetItemsStatistics.Handler(ctx);
        handler.Handle(id).GetAwaiter().GetResult();
    }
}
