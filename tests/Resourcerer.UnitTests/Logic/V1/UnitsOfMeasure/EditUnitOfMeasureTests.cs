using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.UnitsOfMeasure;

public class EditUnitOfMeasureTests : TestsBase
{
    private readonly EditUnitOfMeasure.Handler _sut;
    public EditUnitOfMeasureTests() => _sut = new(_ctx, new(), GetMapster());

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var uom = _forger.Fake<UnitOfMeasure>();
        var request = new V1EditUnitOfMeasure
        {
            Id = uom.Id,
            Name = "Astronomical unit",
            Symbol = "AU"
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.Equal(request.Name, result.Object!.Name),
            () => Assert.Equal(request.Symbol, result.Object!.Symbol)
        );
    }

    [Fact]
    public void UnitNotFound__NotFound()
    {
        // arrange
        _forger.Fake<UnitOfMeasure>();
        var request = new V1EditUnitOfMeasure
        {
            Id = Guid.NewGuid()
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }
}
