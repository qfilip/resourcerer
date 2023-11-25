using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.QueryUtils;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.Functions.Instances;

public class GetInstanceInfoTests : TestsBase
{
    private readonly Func<Instance, DateTime, InstanceInfoDto> _sut;
    public GetInstanceInfoTests()
    {
        _sut = Resourcerer.Logic.Functions.Instances.GetInstanceInfo;
    }

    [Fact]
    public void Item_Bought()
    {
        var boughtEvent1 = Mocker.MockBoughtEvent(_testDbContext, _sand, (ev) =>
        {
            ev.UnitPrice = 1;
            ev.Quantity = 1;
        });
        
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
        var boughtEvent1 = Mocker.MockBoughtEvent(_testDbContext, _sand ,(ev) =>
        {
            ev.UnitPrice = 1;
            ev.Quantity = 1;
        });

        Mocker.MockDeliveredEvent(_testDbContext, boughtEvent1, x =>
        {
            x.CreatedAt = Mocker.Now.AddMonths(1);
        });

        SaveToDb();

        var instance = ExecuteTestQuery().Instances.First();
        
        var expected = new InstanceInfoDto
        {
            InstanceId = instance.Id,
            PendingToArrive = 0,
            PurchaseCost = 1,
            Discards = Array.Empty<DiscardInfoDto>(),
            SellProfit = 0,
            ExpiryDate = instance.ExpiryDate,
            QuantityLeft = 1
        };
        var actual = _sut(instance, Mocker.Now.AddMonths(2));

        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void Item_BoughtAndCancelled_WithRefund()
    {
        var boughtEvent = Mocker.MockBoughtEvent(_testDbContext, _sand, (ev) =>
        {
            ev.UnitPrice = 1;
            ev.Quantity = 1;
        });

        Mocker.MockBoughtCancelledEvent(_testDbContext, boughtEvent, x =>
        {
            x.Reason = "test-reason";
            x.RefundedAmount = 0.5d;
        });

        SaveToDb();

        var instance = ExecuteTestQuery().Instances.First();

        var expected = new InstanceInfoDto
        {
            InstanceId = instance.Id,
            PendingToArrive = 0,
            PurchaseCost = 0.5d,
            SellProfit = 0,
            QuantityLeft = 0
        };
        var actual = _sut(instance, Mocker.Now.AddMonths(2));

        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void Item_Bought_Delivered_Sold()
    {
        var boughtEvent = Mocker.MockBoughtEvent(_testDbContext, _sand, (ev) =>
        {
            ev.UnitPrice = 1;
            ev.Quantity = 2;
        });

        var deliveredEvent = Mocker.MockDeliveredEvent(_testDbContext, boughtEvent, x =>
        {
            x.CreatedAt = Mocker.Now.AddMonths(1);
        });

        Mocker.MockSoldEvent(_testDbContext, boughtEvent, x =>
        {
            x.UnitPrice = 2;
            x.Quantity = 1;
        });

        SaveToDb();

        var instance = ExecuteTestQuery().Instances.First();

        var expected = new InstanceInfoDto
        {
            InstanceId = instance.Id,
            PendingToArrive = 0,
            PurchaseCost = 2,
            SellProfit = 2,
            QuantityLeft = 1,
            Discards = Array.Empty<DiscardInfoDto>(),
            ExpiryDate = instance.ExpiryDate
        };
        var actual = _sut(instance, Mocker.Now.AddMonths(2));

        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void Item_Bought_Delivered_Sold_SellCancelledWithRefund()
    {
        var boughtEvent = Mocker.MockBoughtEvent(_testDbContext, _sand, (ev) =>
        {
            ev.UnitPrice = 1;
            ev.Quantity = 2;
        });

        var deliveredEvent = Mocker.MockDeliveredEvent(_testDbContext, boughtEvent, x =>
        {
            x.CreatedAt = Mocker.Now.AddMonths(1);
        });

        var soldEvent = Mocker.MockSoldEvent(_testDbContext, boughtEvent, x =>
        {
            x.UnitPrice = 2;
            x.Quantity = 1;
        });

        Mocker.MockSoldCancelledEvent(_testDbContext, soldEvent, x =>
        {
            x.RefundedAmount = 2;
        });

        SaveToDb();

        var instance = ExecuteTestQuery().Instances.First();

        var expected = new InstanceInfoDto
        {
            InstanceId = instance.Id,
            PendingToArrive = 0,
            PurchaseCost = 2,
            SellProfit = 0,
            QuantityLeft = boughtEvent.Quantity,
            Discards = Array.Empty<DiscardInfoDto>(),
            ExpiryDate = instance.ExpiryDate
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
