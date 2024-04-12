﻿using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items;

public class GetCompanyItemsTests : TestsBase
{
    private readonly GetCompanyItems.Handler _sut;
    public GetCompanyItemsTests() => _sut = new(_ctx);

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var category = DF.Fake<Category>(_ctx);
        for (var i = 0; i < 10; i++)
            DF.Fake<Item>(_ctx, item =>
            {
                item.Category = category;

                for (var j = 0; j < 10; j++)
                    DF.Fake<Price>(_ctx, p =>
                    {
                        p.Item = item;
                        p.EntityStatus = j == 0 ? eEntityStatus.Active : eEntityStatus.Deleted;
                    });
            });

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(category.Company!.Id).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.Equal(10, result.Object!.Length),
            () =>
            {
                foreach (var item in result.Object!)
                    Assert.True(item.Prices.Length == 1);
            }
        );
    }
}
