using Resourcerer.Logic;
using Resourcerer.Logic.Queries.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0.Queries.Categories;

public class GetAllCompanyCategoriesTests : TestsBase
{
    private readonly GetAllCompanyCategories.Handler _handler;
    public GetAllCompanyCategoriesTests()
    {
        _handler = new GetAllCompanyCategories.Handler(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var company = DF.FakeCompany(_testDbContext);
        Enumerable.Range(0, 2)
            .Select(x => DF.FakeCategory(_testDbContext, x => x.CompanyId = company.Id))
            .ToList()
            .ForEach(parent =>
            {
                DF.FakeCategory(_testDbContext, catg =>
                {
                    catg.CompanyId = company.Id;
                    catg.ParentCategoryId = parent.Id;
                });
            });

        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(company.Id).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
