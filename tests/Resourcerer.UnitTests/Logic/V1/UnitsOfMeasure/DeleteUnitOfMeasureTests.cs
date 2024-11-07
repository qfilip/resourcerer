using Microsoft.EntityFrameworkCore;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.UnitsOfMeasure;

public class DeleteUnitOfMeasureTests : TestsBase
{
    private readonly DeleteUnitOfMeasure.Handler _sut;
    public DeleteUnitOfMeasureTests() => _sut = new(_ctx, GetMapster());

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var uom = _forger.Fake<UnitOfMeasure>();
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(uom.Id).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();

                var entity = _ctx.UnitsOfMeasure
                    .IgnoreQueryFilters()
                    .First(x => x.Id == uom.Id);

                Assert.Equal(eEntityStatus.Deleted, entity.EntityStatus);
            }
        );
    }

    [Fact]
    public void EntityNotFound__NotFound()
    {
        // arrange
        _forger.Fake<UnitOfMeasure>();
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(Guid.NewGuid()).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void HasNonDeletedItems__Rejected()
    {
        // arrange
        var uom = _forger.Fake<UnitOfMeasure>(x =>
        {
            x.Items = Enumerable.Range(0, 3)
                .Select(iter => _forger.Fake<Item>(i =>
                {
                    i.UnitOfMeasure = x;
                    i.EntityStatus = iter % 2 == 0 ? eEntityStatus.Deleted : eEntityStatus.Active;
                }))
                .ToList();
        });
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(uom.Id).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void AllItsItemsDeleted__Ok()
    {
        // arrange
        var uom = _forger.Fake<UnitOfMeasure>(x =>
        {
            x.Items = Enumerable.Range(0, 3)
                .Select(_ => _forger.Fake<Item>(i =>
                {
                    i.UnitOfMeasure = x;
                    i.EntityStatus = eEntityStatus.Deleted;
                }))
                .ToList();
        });
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(uom.Id).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
