using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateItemOrderedEventTests : TestsBase
{
    private readonly CreateInstanceOrderedEvent.Handler _handler;
    public CreateItemOrderedEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var sourceInstance = DF.FakeInstance(_testDbContext, x => x.ExpiryDate = DateTime.UtcNow.AddDays(3));
        var buyerCompany = DF.FakeCompany(_testDbContext);
        _testDbContext.SaveChanges();

        var dto = GetDto(sourceInstance, x =>
        {
            x.BuyerCompanyId = buyerCompany.Id;
            x.ExpectedDeliveryDate = DateTime.UtcNow;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        var entities = _testDbContext.Instances.ToArray();

        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.True(entities.Length == 2);
    }

    [Fact]
    public void When_RequestDto_IsInvalid_Then_ValidationError()
    {
        // arrange
        var dto = GetDto(new Instance(), x =>
        {
            x.InstanceId = Guid.Empty;
            x.SellerCompanyId = Guid.Empty;
            x.BuyerCompanyId = Guid.Empty;
            x.UnitPrice = -1;
            x.UnitsOrdered = -1;
            x.TotalDiscountPercent = -1;
        });

        // act
        var result = _handler.Validate(dto);

        // assert
        Assert.Equal(6, result.Errors.Count);
    }

    [Fact]
    public void When_Item_CanExpire_And_ExpectedDeliveryDate_IsNull_Then_Rejected()
    {
        // arrange
        var srcInstance = DF.FakeInstance(_testDbContext, x => x.ExpiryDate = DateTime.UtcNow.AddDays(3));
        _testDbContext.SaveChanges();

        var dto = GetDto(srcInstance, x => x.ExpectedDeliveryDate = null);

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_Item_CanExpire_And_ExpiryDate_IsLower_Than_ExpectedDeliveryDate_Then_Rejected()
    {
        // arrange
        var srcInstance = DF.FakeInstance(_testDbContext, x => x.ExpiryDate = DateTime.UtcNow.AddDays(3));
        _testDbContext.SaveChanges();
        
        var dto = GetDto(srcInstance, x =>
        {
            x.ExpectedDeliveryDate = srcInstance.ExpiryDate?.AddDays(1);
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_Item_CannotExpire_And_ExpiryDate_Or_ExpectedDeliveryDate_Invalid_Then_Ok()
    {
        // arrange
        var srcInstance = DF.FakeInstance(_testDbContext);
        _testDbContext.SaveChanges();

        var dto = GetDto(srcInstance, x =>
        {
            x.ExpectedDeliveryDate = null;
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
            TotalDiscountPercent = 5,
            UnitPrice = 1,
            UnitsOrdered = 10,
        };

        modifier?.Invoke(dto);

        return dto;
    }
}
