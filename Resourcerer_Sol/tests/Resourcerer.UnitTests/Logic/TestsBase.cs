using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Logic.Queries.Items;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;
using Resourcerer.UnitTests.Utilities.TestDatabaseMocks;
using System.Text.Json;

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

        var sand = ctx.Items.AsNoTracking().Where(x => x.Name == "sand").First();
        var now = Mocker.Now.AddMonths(4);

        Mocker.MockOrderedEvent(ctx, null, sand);
        Mocker.MockOrderCancelledEvent(ctx, null, sand);
        Mocker.MockDeliveredEvent(ctx, x => x.CreatedAt = Mocker.Now.AddMonths(1), sand);

        ctx.SaveChanges();

        var handler = new GetItemStatistics.Handler(ctx);
        var result = handler.Handle((sand.Id, now)).GetAwaiter().GetResult();
    }

    //[Fact]
    //public void SeedCocktailDb()
    //{
    //    var seed = CocktailDbMocker.GetSeed();
    //    var json = JsonSerializer.Serialize(seed);
    //}
}
