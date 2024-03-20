using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1.Commands.UnitsOfMeasure;

public class CreateUnitOfMeasureTests : TestsBase
{
    private readonly CreateUnitOfMeasure.Handler _handler;

    public CreateUnitOfMeasureTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Ok()
    {
        // arrange
        var company = DF.FakeCompany(_testDbContext);
        var dto = new V1CreateUnitOfMeasure
        {
            CompanyId = company.Id,
            Name = "tests",
            Symbol = "ts"
        };

        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_CompanyNotFound_Then_Rejected()
    {
        // arrange
        var company = DF.FakeCompany(_testDbContext);
        var dto = new V1CreateUnitOfMeasure
        {
            CompanyId = Guid.NewGuid(),
            Name = "tests",
            Symbol = "ts"
        };
        
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
