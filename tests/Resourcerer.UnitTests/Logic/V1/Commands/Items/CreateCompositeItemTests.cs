﻿using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1.Commands.Items;

public class CreateCompositeItemTests : TestsBase
{
    private readonly CreateCompositeItem.Handler _handler;
    public CreateCompositeItemTests()
    {
        _handler = new(_ctx, new());
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var dto = GetDto();
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void ElementWithSameNameAndCategory_Exsts__Rejected()
    {
        // arrange
        var existingElement = DF.Fake<Item>(_ctx);
        var dto = GetDto(x =>
        {
            x.Name = existingElement.Name;
            x.CategoryId = existingElement.CategoryId;
        });
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void Category_NotFound__NotFound()
    {
        // arrange
        var dto = GetDto(x => x.CategoryId = Guid.NewGuid());
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void UnitOfMeasure_NotFound_Then__Rejected()
    {
        // arrange
        var dto = GetDto(x => x.UnitOfMeasureId = Guid.NewGuid());
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void RequiredElements_NotFound_Then__Rejected()
    {
        // arrange
        var dto = GetDto(x => x.ExcerptMap = new Dictionary<Guid, double>
        {
            { Guid.NewGuid(), 1 },
            { Guid.NewGuid(), 2 }
        });
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    private V1CreateCompositeItem GetDto(Action<V1CreateCompositeItem>? modifier = null)
    {
        var dto = new V1CreateCompositeItem
        {
            Name = "test",
            CategoryId = DF.Fake<Category>(_ctx).Id,
            UnitOfMeasureId = DF.Fake<UnitOfMeasure>(_ctx).Id,
            UnitPrice = 2,
            PreparationTimeSeconds = 2,
            ExpirationTimeSeconds = 2,
            ExcerptMap = new Dictionary<Guid, double>
            {
                { DF.Fake<Item>(_ctx).Id, 1 },
                { DF.Fake<Item>(_ctx).Id, 2 }
            }
        };

        modifier?.Invoke(dto);

        return dto;
    }
}
