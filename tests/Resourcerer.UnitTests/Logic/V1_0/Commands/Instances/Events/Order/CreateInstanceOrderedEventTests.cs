using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Instances.Events.Order;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0.Commands.Instances;

public class CreateInstanceOrderedEventTests : TestsBase
{
    private readonly CreateInstanceOrderedEvent.Handler _handler;
    public CreateInstanceOrderedEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var derivedInstanceItem = DF.FakeItem(_testDbContext);
        var sourceInstance = DF.FakeInstance(_testDbContext, x =>
        {
            x.Quantity = 1;
        });
        _testDbContext.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.BuyerCompanyId = derivedInstanceItem.Category!.CompanyId;
            x.SellerCompanyId = sourceInstance.OwnerCompanyId;
            x.DerivedInstanceItemId = derivedInstanceItem.Id;
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        var instance = _testDbContext.Instances.First(x => x.Id == sourceInstance.Id);
        var entities = _testDbContext.Instances.ToArray();

        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.True(entities.Length == 2);
        Assert.True(instance.OrderedEvents.Any());
    }

    [Fact]
    public void When_RequestDto_IsInvalid_Then_ValidationError()
    {
        // arrange
        var dto = new InstanceOrderRequestDto
        {
            InstanceId = Guid.Empty,
            SellerCompanyId = Guid.Empty,
            BuyerCompanyId = Guid.Empty,
            UnitPrice = -1,
            UnitsOrdered = -1,
            TotalDiscountPercent = -1
        };

        // act
        var result = _handler.Validate(dto);

        // assert
        Assert.Equal(6, result.Errors.Count);
    }

    [Fact]
    public void When_BuyerNotFound_Then_Rejected()
    {
        var sourceInstance = DF.FakeInstance(_testDbContext, x =>
        {
            x.Quantity = 1;
        });
        _testDbContext.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.BuyerCompanyId = Guid.NewGuid();
            x.SellerCompanyId = sourceInstance.OwnerCompanyId;
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_SellerNotFound_Then_Rejected()
    {
        var buyerCompany = DF.FakeCompany(_testDbContext);
        var sourceInstance = DF.FakeInstance(_testDbContext, x =>
        {
            x.Quantity = 1;
        });
        _testDbContext.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = Guid.NewGuid();
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_DerivedInstanceItem_NotFound_Then_Rejected()
    {
        // arrange
        var derivedInstanceItem = DF.FakeItem(_testDbContext);
        var sourceInstance = DF.FakeInstance(_testDbContext, x =>
        {
            x.Quantity = 1;
        });
        _testDbContext.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.BuyerCompanyId = derivedInstanceItem.Category!.CompanyId;
            x.SellerCompanyId = sourceInstance.OwnerCompanyId;
            x.DerivedInstanceItemId = Guid.NewGuid();
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_DerivedInstanceItem_HasDifferentBuyerCompany_Then_Rejected()
    {
        // arrange
        var buyerCompany = DF.FakeCompany(_testDbContext);
        var derivedInstanceItem = DF.FakeItem(_testDbContext, x =>
        {
            x.Category!.CompanyId = DF.FakeCompany(_testDbContext).Id;
        });
        var sourceInstance = DF.FakeInstance(_testDbContext, x =>
        {
            x.Quantity = 1;
        });
        _testDbContext.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompanyId;
            x.DerivedInstanceItemId = derivedInstanceItem.Id;
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_InstanceNotFound_Then_Rejected()
    {
        var buyerCompany = DF.FakeCompany(_testDbContext);
        var sourceInstance = DF.FakeInstance(_testDbContext, x =>
        {
            x.Quantity = 1;
        });
        _testDbContext.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.InstanceId = Guid.NewGuid();
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompanyId;
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_ExpectedDeliveryDate_NotSet_And_Instance_HasExpiryDate_Then_Rejected()
    {
        var buyerCompany = DF.FakeCompany(_testDbContext);
        var sourceInstance = DF.FakeInstance(_testDbContext, x =>
        {
            x.Quantity = 1;
            x.ExpiryDate = DateTime.UtcNow;
        });
        _testDbContext.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.InstanceId = sourceInstance.Id;
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompanyId;
            x.UnitsOrdered = 1;
            x.ExpectedDeliveryDate = null;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_DeliveryDate_LargerOrEqualTo_InstanceExpiryDate_Then_Rejected()
    {
        var buyerCompany = DF.FakeCompany(_testDbContext);
        var sourceInstance = DF.FakeInstance(_testDbContext, x =>
        {
            x.Quantity = 1;
            x.ExpiryDate = DateTime.UtcNow;
        });
        _testDbContext.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.InstanceId = sourceInstance.Id;
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompanyId;
            x.UnitsOrdered = 1;
            x.ExpectedDeliveryDate = sourceInstance.ExpiryDate?.AddSeconds(1);
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        var entities = _testDbContext.Instances.ToArray();

        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_UnitsOrdered_LargerThan_UnitsInStock_Then_Rejected()
    {
        var buyerCompany = DF.FakeCompany(_testDbContext);
        var sourceInstance = DF.FakeInstance(_testDbContext, x =>
        {
            x.Quantity = 2;
            x.ExpiryDate = DateTime.UtcNow;
            x.OrderedEvents = new List<InstanceOrderedEvent>
            {
                DF.FakeOrderedEvent(_testDbContext, ev =>
                {
                    ev.Quantity = 1;
                    ev.SentEvent = DF.FakeSentEvent();
                })
            };
            x.DiscardedEvents = new List<InstanceDiscardedEvent>
            {
                DF.FakeDiscardedEvent(x, x => x.Quantity = 1)
            };
        });
        _testDbContext.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.InstanceId = sourceInstance.Id;
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompanyId;
            x.UnitsOrdered = 2;
            x.ExpectedDeliveryDate = sourceInstance.ExpiryDate?.AddSeconds(1);
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_UnitsOrdered_LessOrEqualTo_UnitsInStock_Then_Ok()
    {
        var buyerCompany = DF.FakeCompany(_testDbContext);
        var sourceInstance = DF.FakeInstance(_testDbContext, x =>
        {
            x.Quantity = 4;
            x.ExpiryDate = DateTime.UtcNow;
            x.OrderedEvents = new List<InstanceOrderedEvent>
            {
                DF.FakeOrderedEvent(_testDbContext, ev =>
                {
                    ev.Quantity = 1;
                    ev.SentEvent = DF.FakeSentEvent();
                })
            };
            x.DiscardedEvents = new List<InstanceDiscardedEvent>
            {
                DF.FakeDiscardedEvent(x, x => x.Quantity = 1)
            };
        });
        _testDbContext.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.InstanceId = sourceInstance.Id;
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompanyId;
            x.UnitsOrdered = 2;
            x.ExpectedDeliveryDate = DateTime.UtcNow.AddSeconds(-1);
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    private InstanceOrderRequestDto GetDto(Instance sourceInstance, Action<InstanceOrderRequestDto>? modifier = null)
    {
        var dto = new InstanceOrderRequestDto()
        {
            InstanceId = sourceInstance.Id,
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(1),
            SellerCompanyId = sourceInstance.OwnerCompany!.Id,
            BuyerCompanyId = Guid.Empty,
            TotalDiscountPercent = 0,
            UnitPrice = 1,
            UnitsOrdered = 1,
        };

        modifier?.Invoke(dto);

        return dto;
    }
}
