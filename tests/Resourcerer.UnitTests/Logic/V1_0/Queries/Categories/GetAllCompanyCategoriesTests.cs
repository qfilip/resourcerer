using Resourcerer.Logic;
using Resourcerer.Logic.Queries.V1;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0.Queries.Categories;

public class GetAllCompanyCategoriesTests : TestsBase
{
    private readonly GetAllCompanyCategories.Handler _handler;
    public GetAllCompanyCategoriesTests()
    {
        _handler = new GetAllCompanyCategories.Handler(_ctx);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var company = DF.FakeCompany(_ctx);
        Enumerable.Range(0, 2)
            .Select(x => DF.FakeCategory(_ctx, x => x.CompanyId = company.Id))
            .ToList()
            .ForEach(parent =>
            {
                DF.FakeCategory(_ctx, catg =>
                {
                    catg.CompanyId = company.Id;
                    catg.ParentCategoryId = parent.Id;
                });
            });

        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(company.Id).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
