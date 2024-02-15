using FakeItEasy;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly AppDbContext _testDbContext;
    protected readonly Item _sand;
    protected readonly Item _meat;

    public TestsBase()
    {
        _testDbContext = new ContextCreator().GetTestDbContext();
        DF.MockDbData(_testDbContext);

        _sand = _testDbContext.Items.First(x => x.Name == "sand");
        _meat = _testDbContext.Items.First(x => x.Name == "meat");
    }

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();

    //[Fact]
    //public void Test()
    //{
    //    var ctx = new ContextCreator().GetTestDbContext();
    //    Mocker.MockDbData(ctx);

    //    var sand = ctx.Items.AsNoTracking().Where(x => x.Name == "sand").First();
    //    var now = Mocker.Now.AddMonths(4);

    //    // bought and delivered
    //    var boughtEvent1 = Mocker.MockBoughtEvent(ctx, null, sand);
    //    Mocker.MockDeliveredEvent(ctx, x =>
    //    {
    //        x.InstanceBoughtEvent = boughtEvent1;
    //        x.CreatedAt = Mocker.Now.AddMonths(1);
    //    }, sand);

    //    // bought and delivered
    //    var boughtEvent2 = Mocker.MockBoughtEvent(ctx, null, sand);
    //    Mocker.MockDeliveredEvent(ctx, x =>
    //    {
    //        x.InstanceBoughtEvent = boughtEvent2;
    //        x.CreatedAt = Mocker.Now.AddMonths(1);
    //    }, sand);

    //    // bought but cancelled
    //    var boughtEvent3 = Mocker.MockBoughtEvent(ctx, null, sand);
    //    Mocker.MockBoughtCancelledEvent(ctx, x =>
    //    {
    //        x.InstanceBoughtEvent = boughtEvent3;
    //    }, sand);

    //    Mocker.MockSoldEvent
        

    //    ctx.SaveChanges();

    //    var handler = new GetItemStatistics.Handler(ctx);
    //    var result = handler.Handle((sand.Id, now)).GetAwaiter().GetResult();
    //}

    //[Fact]
    //public void SeedCocktailDb()
    //{
    //    var seed = CocktailDbMocker.GetSeed();
    //    var json = JsonSerializer.Serialize(seed);
    //}
}
