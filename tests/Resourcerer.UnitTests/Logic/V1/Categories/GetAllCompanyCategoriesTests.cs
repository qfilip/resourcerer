using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Logic;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Categories;

public class GetAllCompanyCategoriesTests : TestsBase
{
    private readonly GetAllCompanyCategories.Handler _handler;
    public GetAllCompanyCategoriesTests()
    {
        _handler = new GetAllCompanyCategories.Handler(_ctx, new());
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var company = DF.Fake<Company>(_ctx);
        Enumerable.Range(0, 2)
            .Select(x => DF.Fake<Category>(_ctx, x => x.Company = company))
            .ToList()
            .ForEach(parent =>
            {
                DF.Fake<Category>(_ctx, catg =>
                {
                    catg.Company = company;
                    catg.ParentCategory = parent;
                });
            });

        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(company.Id).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
