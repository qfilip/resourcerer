using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.Queries.Items;

public class ItemEventsTests : TestsBase
{
    private readonly Item _sand;
    public ItemEventsTests()
    {
        Mocker.MockDbData(_testDbContext);
        _sand = _testDbContext.Items.FirstOrDefault(x => x.Name == "sand")!;
    }

    [Fact]
    public void Given_Item_Bought_And_Delivered_Then_OneItemInstance()
    {
        ArrangeDb(() =>
        {
            var boughtEvent1 = Mocker.MockBoughtEvent(_testDbContext, null, _sand);
            Mocker.MockDeliveredEvent(_testDbContext, x =>
            {
                x.InstanceBoughtEvent = boughtEvent1;
                x.CreatedAt = Mocker.Now.AddMonths(1);
            }, _sand);
        });

        var instances = _testDbContext.Instances.Where(x => x.ItemId == _sand.Id).ToList();
        Assert.True(instances.Count == 1);
    }

    private void ArrangeDb(Action arrange)
    {
        arrange();
        _testDbContext.SaveChanges();
        _testDbContext.ChangeTracker.Clear();
    }
}
