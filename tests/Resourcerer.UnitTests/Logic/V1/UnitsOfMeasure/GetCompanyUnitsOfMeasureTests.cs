using FakeItEasy;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.UnitsOfMeasure;

public class GetCompanyUnitsOfMeasureTests : TestsBase
{
    private readonly GetCompanyUnitsOfMeasure.Handler _sut;
    public GetCompanyUnitsOfMeasureTests() => _sut = new(_ctx);

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var company1 = _forger.Fake<Company>();
        var company2 = _forger.Fake<Company>();
        for (var i = 0; i < 10; i++)
        {
            var company = i % 2 == 0 ? company1 : company2;
            _forger.Fake<UnitOfMeasure>(x => x.Company = company);
        }

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(company1.Id).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.True(result.Object!.Where(x => x.CompanyId == company1.Id).Count() == 5)
        );
    }
}
