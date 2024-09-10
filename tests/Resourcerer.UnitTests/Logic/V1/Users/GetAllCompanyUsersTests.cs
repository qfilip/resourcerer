using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Users;

public class GetAllCompanyUsersTests : TestsBase
{
    private readonly GetAllCompanyUsers.Handler _sut;
    public GetAllCompanyUsersTests() => _sut = new(_ctx, new());

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var company = _forger.Fake<Company>();
        var numOfUsers = 3;
        for (var i = 0; i < numOfUsers; i++)
            _forger.Fake<AppUser>(x => x.Company = company);

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(company.Id).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.True(result.Object!.Length == numOfUsers),
            () => Assert.All(result.Object!, x => string.IsNullOrEmpty(x.Password))
        );
    }
}
