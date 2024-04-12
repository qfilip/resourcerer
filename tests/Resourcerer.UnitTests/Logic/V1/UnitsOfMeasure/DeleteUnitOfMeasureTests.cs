using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.UnitsOfMeasure;

public class DeleteUnitOfMeasureTests : TestsBase
{
    private readonly DeleteUnitOfMeasure.Handler _sut;
    public DeleteUnitOfMeasureTests() => _sut = new(_ctx);

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var uom = DF.Fake<UnitOfMeasure>(_ctx);
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
        DF.Fake<UnitOfMeasure>(_ctx);
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(Guid.NewGuid()).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }
}
