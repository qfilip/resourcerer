using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.UnitsOfMeasure;

public class CreateUnitOfMeasureTests : TestsBase
{
    private readonly CreateUnitOfMeasure.Handler _handler;

    public CreateUnitOfMeasureTests()
    {
        _handler = new(_ctx, new(), GetMapster());
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var company = _forger.Fake<Company>();
        var dto = new V1CreateUnitOfMeasure
        {
            CompanyId = company.Id,
            Name = "tests",
            Symbol = "ts"
        };

        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void CompanyNotFound__NotFound()
    {
        // arrange
        _forger.Fake<Company>();
        var dto = new V1CreateUnitOfMeasure
        {
            CompanyId = Guid.NewGuid(),
            Name = "tests",
            Symbol = "ts"
        };

        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void CompanyNotFound__Rejected()
    {
        // arrange
        var uom = _forger.Fake<UnitOfMeasure>(x =>
        {
            x.Name = "Unit";
            x.Symbol = "u";
        });

        var dto = new V1CreateUnitOfMeasure
        {
            CompanyId = uom.CompanyId,
            Name = uom.Name,
            Symbol = uom.Symbol
        };

        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Rejected, result.Status),
            () => Assert.True(result.Errors.Length == 2)
        );
    }
}
