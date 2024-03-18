using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.QueryUtils;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances;
using Resourcerer.UnitTests.Utilities.Faker;
using System.Linq.Expressions;

namespace Resourcerer.UnitTests.Logic.V1_0.Functions.Instances;

public class GetInstanceInfoTests : TestsBase
{
    private readonly Func<Instance, DateTime, InstanceInfoDto> _sut;
    public GetInstanceInfoTests()
    {
        _sut = Resourcerer.Logic.V1_0.Functions.Instances.GetInstanceInfo;
    }

    [Fact]
    public void Item_Ordered()
    {
        var orderEvent = DF.FakeOrderedEvent(_testDbContext, (ev) =>
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
        var actual = _sut(instance, DF.Now.AddMonths(2));

        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void Item_BoughtAndDelivered()
    {
        var orderEvent = DF.FakeOrderedEvent(_testDbContext, (ev) =>
        {
            ev.UnitPrice = 1;
            ev.Quantity = 1;
            ev.DeliveredEvent = DF.FakeDeliveredEvent(x =>
            {
                x.CreatedAt = DF.Now.AddMonths(1);
            });
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
        var actual = _sut(instance, DF.Now.AddMonths(2));

        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void Item_BoughtAndCancelled_WithRefund()
    {
        var orderEvent = DF.FakeOrderedEvent(_testDbContext, (ev) =>
        {
            ev.UnitPrice = 1;
            ev.Quantity = 1;
            ev.OrderCancelledEvent = DF.FakeOrderCancelledEvent(c =>
            {
                c.Reason = "test-reason";
                c.RefundedAmount = 0.5d;
            });
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
        var actual = _sut(instance, DF.Now.AddMonths(2));

        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void Item_Bought_Delivered_Sold()
    {
        //var orderEvent = DF.FakeOrderedEvent(_testDbContext, (ev) =>
        //{
        //    ev.UnitPrice = 1;
        //    ev.Quantity = 2;
        //});

        //var deliveredEvent = DF.FakeDeliveredEvent(orderEvent, x =>
        //{
        //    x.CreatedAt = DF.Now.AddMonths(1);
        //});

        //DF.FakeSoldEvent(_testDbContext, boughtEvent, x =>
        //{
        //    x.UnitPrice = 2;
        //    x.Quantity = 1;
        //});

        //SaveToDb();

        //var instance = ExecuteTestQuery().Instances.First();

        //var expected = new InstanceInfoDto
        //{
        //    InstanceId = instance.Id,
        //    PendingToArrive = 0,
        //    PurchaseCost = 2,
        //    SellProfit = 2,
        //    QuantityLeft = 1,
        //    Discards = Array.Empty<DiscardInfoDto>(),
        //    ExpiryDate = instance.ExpiryDate
        //};
        //var actual = _sut(instance, DF.Now.AddMonths(2));

        //Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void Item_Bought_Delivered_Sold_SellCancelledWithPenalty()
    {
        //var boughtEvent = DF.MockOrderedEvent(_testDbContext, _sand, (ev) =>
        //{
        //    ev.UnitPrice = 1;
        //    ev.Quantity = 2;
        //});

        //var deliveredEvent = DF.MockDeliveredEvent(_testDbContext, boughtEvent, x =>
        //{
        //    x.CreatedAt = DF.Now.AddMonths(1);
        //});

        //var soldEvent = DF.MockSoldEvent(_testDbContext, boughtEvent, x =>
        //{
        //    x.UnitPrice = 2;
        //    x.Quantity = 1;
        //});

        //var sellCancelledEvent = DF.MockSellCancelledEvent(_testDbContext, soldEvent, x =>
        //{
        //    x.RefundedAmount = 2.2;
        //});

        //SaveToDb();

        //var instance = ExecuteTestQuery().Instances.First();

        //var expected = new InstanceInfoDto
        //{
        //    InstanceId = instance.Id,
        //    PendingToArrive = 0,
        //    PurchaseCost = 2,
        //    SellProfit = 0,
        //    SellCancellationsPenaltyDifference = 0.2f,
        //    QuantityLeft = boughtEvent.Quantity,
        //    Discards = Array.Empty<DiscardInfoDto>(),
        //    ExpiryDate = instance.ExpiryDate
        //};
        //var actual = _sut(instance, DF.Now.AddMonths(2));

        //Assert.Equivalent(expected, actual);
    }

    private Item ExecuteTestQuery(Expression<Func<Item, bool>>? predicate = null)
    {
        if (predicate == null)
        {
            //predicate = x => x.Id == _sand.Id;
        }

        var query = _testDbContext
            .Items
            .Where(predicate);

        // query = ItemQueryUtils.IncludeInstanceEvents(query);

        return query.First();
    }

    private void SaveToDb()
    {
        _testDbContext.SaveChanges();
        _testDbContext.ChangeTracker.Clear();
    }
}
