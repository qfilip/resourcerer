﻿using Microsoft.EntityFrameworkCore;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Instances.Events.Order;

public class CreateInstanceOrderedEventTests : TestsBase
{
    private readonly CreateInstanceOrderedEvent.Handler _handler;
    public CreateInstanceOrderedEventTests()
    {
        _handler = new(_ctx);
    }

    [Fact]
    public void WithDerivedInstanceItemMapping__Ok()
    {
        // arrange
        var derivedInstanceItem = _forger.Fake<Item>();
        var sourceInstance = _forger.Fake<Instance>(x =>
        {
            x.Quantity = 1;
        });
        _ctx.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.BuyerCompanyId = derivedInstanceItem.Category!.Company!.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompany!.Id;
            x.DerivedInstanceItemId = derivedInstanceItem.Id;
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var srcInstance = _ctx.Instances
                    .Include(x => x.OrderedEvents)
                    .First(x => x.Id == sourceInstance.Id);
                var dervInstance = _ctx.Instances.First(x => x.ItemId == derivedInstanceItem.Id);
                var entities = _ctx.Instances.ToArray();
                Assert.True(entities.Length == 2);
                Assert.True(srcInstance.OrderedEvents.Any());
            }
        );
    }

    [Fact]
    public void WithoutDerivedInstanceItemMapping__Ok()
    {
        // arrange
        var buyerCompany = _forger.Fake<Company>();
        var sourceInstance = _forger.Fake<Instance>(x =>
        {
            x.Quantity = 1;
        });
        _ctx.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompany!.Id;
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var srcInstance = _ctx.Instances
                    .Include(x => x.OrderedEvents)
                    .First(x => x.Id == sourceInstance.Id);
                var dervInstance = _ctx.Instances.First(x => x.ItemId == sourceInstance.ItemId);
                var entities = _ctx.Instances.ToArray();
                Assert.True(entities.Length == 2);
                Assert.True(srcInstance.OrderedEvents.Any());
            }
        );

    }

    [Fact]
    public void RequestDto_IsInvalid__ValidationError()
    {
        // arrange
        var dto = new V1InstanceOrderCreateCommand
        {
            InstanceId = Guid.Empty,
            SellerCompanyId = Guid.Empty,
            BuyerCompanyId = Guid.Empty,
            UnitPrice = -1,
            UnitsOrdered = -1,
            TotalDiscountPercent = -1
        };

        // act
        var validator = new CreateInstanceOrderedEvent.Validator();
        var result = validator.Validate(dto);

        // assert
        Assert.Equal(6, result.Errors.Count);
    }

    [Fact]
    public void BuyerNotFound__Rejected()
    {
        var sourceInstance = _forger.Fake<Instance>(x =>
        {
            x.Quantity = 1;
        });
        _ctx.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.BuyerCompanyId = Guid.NewGuid();
            x.SellerCompanyId = sourceInstance.OwnerCompany!.Id;
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void SellerNotFound__Rejected()
    {
        var buyerCompany = _forger.Fake<Company>();
        var sourceInstance = _forger.Fake<Instance>(x =>
        {
            x.Quantity = 1;
        });
        _ctx.SaveChanges();

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
    public void DerivedInstanceItem_NotFound__Rejected()
    {
        // arrange
        var derivedInstanceItem = _forger.Fake<Item>();
        var sourceInstance = _forger.Fake<Instance>(x =>
        {
            x.Quantity = 1;
        });
        _ctx.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.BuyerCompanyId = derivedInstanceItem.Category!.CompanyId;
            x.SellerCompanyId = sourceInstance.OwnerCompany!.Id;
            x.DerivedInstanceItemId = Guid.NewGuid();
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void DerivedInstanceItem_HasDifferentBuyerCompany__Rejected()
    {
        // arrange
        var buyerCompany = _forger.Fake<Company>();
        var derivedInstanceItem = _forger.Fake<Item>(x =>
        {
            x.Category!.Company = _forger.Fake<Company>();
        });
        var sourceInstance = _forger.Fake<Instance>(x =>
        {
            x.Quantity = 1;
        });
        _ctx.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompany!.Id;
            x.DerivedInstanceItemId = derivedInstanceItem.Id;
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void InstanceNotFound__Rejected()
    {
        var buyerCompany = _forger.Fake<Company>();
        var sourceInstance = _forger.Fake<Instance>(x =>
        {
            x.Quantity = 1;
        });
        _ctx.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.InstanceId = Guid.NewGuid();
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompany!.Id;
            x.UnitsOrdered = 1;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void ExpectedDeliveryDate_NotSet_And_Instance_HasExpiryDate__Rejected()
    {
        var buyerCompany = _forger.Fake<Company>();
        var sourceInstance = _forger.Fake<Instance>(x =>
        {
            x.Quantity = 1;
            x.ExpiryDate = DateTime.UtcNow;
        });
        _ctx.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.InstanceId = sourceInstance.Id;
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompany!.Id;
            x.UnitsOrdered = 1;
            x.ExpectedDeliveryDate = null;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void DeliveryDate_LargerOrEqualTo_InstanceExpiryDate__Rejected()
    {
        var buyerCompany = _forger.Fake<Company>();
        var sourceInstance = _forger.Fake<Instance>(x =>
        {
            x.Quantity = 1;
            x.ExpiryDate = DateTime.UtcNow;
        });
        _ctx.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.InstanceId = sourceInstance.Id;
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompany!.Id;
            x.UnitsOrdered = 1;
            x.ExpectedDeliveryDate = sourceInstance.ExpiryDate?.AddSeconds(1);
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        var entities = _ctx.Instances.ToArray();

        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void UnitsOrdered_LargerThan_UnitsInStock__Rejected()
    {
        var buyerCompany = _forger.Fake<Company>();
        var sourceInstance = _forger.Fake<Instance>(x =>
        {
            x.Quantity = 2;
            x.ExpiryDate = DateTime.UtcNow;
        });
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(ev => ev.Instance = sourceInstance);
        var discardEvent = _forger.Fake<InstanceDiscardedEvent>(ev => ev.Instance = sourceInstance);

        _ctx.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.InstanceId = sourceInstance.Id;
            x.BuyerCompanyId = buyerCompany.Id;
            x.SellerCompanyId = sourceInstance.OwnerCompany!.Id;
            x.UnitsOrdered = 2;
            x.ExpectedDeliveryDate = sourceInstance.ExpiryDate?.AddSeconds(1);
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void UnitsOrdered_LessOrEqualTo_UnitsInStock__Ok()
    {
        var buyerCompany = _forger.Fake<Company>();
        var sourceInstance = _forger.Fake<Instance>(x =>
        {
            x.Quantity = 4;
            x.ExpiryDate = DateTime.UtcNow;
        });
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x =>
        {
            x.Instance = sourceInstance;
            x.SentEvent = AppDbJsonField.CreateKeyless(() => new InstanceOrderSentEvent());
        });
        var discardEvent = _forger.Fake<InstanceDiscardedEvent>(x => x.Instance = sourceInstance);

        _ctx.SaveChanges();

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

    private V1InstanceOrderCreateCommand GetDto(Instance sourceInstance, Action<V1InstanceOrderCreateCommand>? modifier = null)
    {
        var dto = new V1InstanceOrderCreateCommand()
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
