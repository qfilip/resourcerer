﻿using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateInstanceOrderedEventTests : TestsBase
{
    private readonly CreateItemOrderedEvent.Handler _handler;
    public CreateInstanceOrderedEventTests()
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
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        var entity = _testDbContext.InstanceBoughtEvents
            .Include(x => x.Instance)
            .Single();

        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.NotNull(entity.Instance);
    }

    [Fact]
    public void When_Item_CanExpire_And_ExpectedDeliveryDate_IsNull_Then_ValidationError()
    {
        // arrange
        var dto = GetDto(x => x.ExpectedDeliveryDate = null);
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_Item_CanExpire_And_ExpiryDate_IsNull_Then_ValidationError()
    {
        // arrange
        var dto = GetDto(x => x.ExpiryDate = null);
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_Item_CanExpire_And_ExpiryDate_IsLower_Than_ExpectedDeliveryDate_Then_ValidationError()
    {
        // arrange
        var dto = GetDto(x =>
        {
            x.ExpiryDate = DateTime.Now;
            x.ExpectedDeliveryDate = DateTime.Now.AddDays(1);
        });
        _testDbContext.SaveChanges();

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
        _testDbContext.SaveChanges();

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
            TotalDiscountPercent = 5,
            UnitPrice = 1,
            UnitsOrdered = 10,
        };

        modifier?.Invoke(dto);

        return dto;
    }
}
