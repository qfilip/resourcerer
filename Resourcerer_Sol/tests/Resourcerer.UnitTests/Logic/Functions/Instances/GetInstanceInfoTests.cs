using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.QueryUtils;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.Functions.Instances;

public class GetInstanceInfoTests : TestsBase
{
    private readonly Item _sand;
    private readonly Func<Instance, DateTime, InstanceInfoDto> _sut;
    public GetInstanceInfoTests()
    {
        Mocker.MockDbData(_testDbContext);
        _sand = _testDbContext.Items.FirstOrDefault(x => x.Name == "sand")!;
        _sut = Resourcerer.Logic.Functions.Instances.GetInstanceInfo;
    }

    [Fact]
    public void Item_Bought()
    {
        var boughtEvent1 = Mocker.MockBoughtEvent(_testDbContext, (ev) =>
        {
            ev.UnitPrice = 1;
            ev.Quantity = 1;
        }, _sand);
        
        SaveToDb();

        var instance = ExecuteTestQuery().Instances.First();

        var expected = new InstanceInfoDto
        {
            InstanceId = instance.Id,
            PendingToArrive = 1,
            PurchaseCost = 1
        };
        var actual = _sut(instance, Mocker.Now.AddMonths(2));

        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void Item_BoughtAndDelivered()
    {
        var boughtEvent1 = Mocker.MockBoughtEvent(_testDbContext, (ev) =>
        {
            ev.UnitPrice = 1;
            ev.Quantity = 1;
        }, _sand);

        Mocker.MockDeliveredEvent(_testDbContext, x =>
        {
            x.InstanceBoughtEvent = boughtEvent1;
            x.CreatedAt = Mocker.Now.AddMonths(1);
        }, _sand);

        SaveToDb();

        var instance = ExecuteTestQuery().Instances.First();
        
        var expected = new InstanceInfoDto
        {
            InstanceId = instance.Id,
            PendingToArrive = 1,
            PurchaseCost = 1,
            Discards = Array.Empty<DiscardInfoDto>(),
            SellProfit = 0,
            ExpiryDate = instance.ExpiryDate,
            QuantityLeft = 1
        };
        var actual = _sut(instance, Mocker.Now.AddMonths(2));

        Assert.Equivalent(expected, actual);
    }

    private Item ExecuteTestQuery()
    {
        var query = _testDbContext
            .Items
            .Where(x => x.Id == _sand.Id);

        query = ItemQueryUtils.IncludeInstanceEvents(query);

        return query.First(x => x.Id == _sand.Id);
    }

    private void SaveToDb()
    {
        _testDbContext.SaveChanges();
        _testDbContext.ChangeTracker.Clear();
    }
}
