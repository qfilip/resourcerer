using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
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
        var company = _forger.Fake<Company>();
        Enumerable.Range(0, 2)
            .Select(x => _forger.Fake<Category>(x => x.Company = company))
            .ToList()
            .ForEach(parent =>
            {
                _forger.Fake<Category>(catg =>
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
