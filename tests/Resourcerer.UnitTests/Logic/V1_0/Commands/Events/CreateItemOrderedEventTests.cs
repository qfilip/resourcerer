using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateItemOrderedEventTests : TestsBase
{
    private readonly CreateItemOrderedEvent.Handler _handler;
    public CreateItemOrderedEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var dto = GetDto(x =>
        {
            x.ExpiryDate = DateTime.UtcNow.AddDays(3);
            x.ExpectedDeliveryDate = DateTime.UtcNow;
        });

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        var entity = _testDbContext.ItemOrderedEvents
            .Include(x => x.Instance)
            .Single();

        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.NotNull(entity.Instance);
    }

    [Fact]
    public void When_RequestDto_IsInvalid_Then_ValidationError()
    {
        // arrange
        var dto = GetDto(x =>
        {
            x.ItemId = Guid.Empty;
            x.Seller = null;
            x.Buyer = null;
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
        var dto = GetDto(x => x.ExpectedDeliveryDate = null);

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_Item_CanExpire_And_ExpiryDate_IsNull_Then_Rejected()
    {
        // arrange
        var dto = GetDto(x => x.ExpiryDate = null);

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_Item_CanExpire_And_ExpiryDate_IsLower_Than_ExpectedDeliveryDate_Then_Rejected()
    {
        // arrange
        var dto = GetDto(x =>
        {
            x.ExpiryDate = DateTime.Now;
            x.ExpectedDeliveryDate = DateTime.Now.AddDays(1);
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
        var dto = GetDto(x =>
        {
            x.ExpiryDate = null;
            x.ExpectedDeliveryDate = null;
        },
        i => i.ExpirationTimeSeconds = null);

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    private ItemOrderedEventDto GetDto(Action<ItemOrderedEventDto>? modifier = null, Action<Item>? itemModifier = null)
    {
        var dto = new ItemOrderedEventDto()
        {
            ItemId = Mocker.MockItem(_testDbContext, itemModifier).Id,
            ExpiryDate = DateTime.UtcNow.AddDays(5),
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(1),
            Seller = "seller",
            Buyer = "buyer",
            TotalDiscountPercent = 5,
            UnitPrice = 1,
            UnitsOrdered = 10,
        };

        modifier?.Invoke(dto);

        return dto;
    }
}
