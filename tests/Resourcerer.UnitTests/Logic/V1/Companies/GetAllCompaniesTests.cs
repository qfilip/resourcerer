using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Companies;

public class GetAllCompaniesTests : TestsBase
{
    private readonly GetAllCompanies.Handler _sut;
    public GetAllCompaniesTests()
    {
        _sut = new(_ctx);
    }

    [Fact]
    public void BasicTest()
    {
        // arrange
        var fakeCount = 3;

        for(int i = 0; i < fakeCount; i++)
            _forger.Fake<Company>();

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(Unit.New).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.Equal(fakeCount, result.Object!.Length)
        );
    }
}
