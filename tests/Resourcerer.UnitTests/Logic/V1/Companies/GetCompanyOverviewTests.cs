using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Companies;

public class GetCompanyOverviewTests : TestsBase
{
    private readonly GetCompanyOverview.Handler _sut;
    public GetCompanyOverviewTests()
    {
        _sut = new(_ctx);
    }

    [Fact]
    public void BasicTest()
    {
        var companyId = FakeTestData(_ctx);

        var result = _sut.Handle(companyId).Await();
        
        Assert.NotNull(result);
    }

    private static Guid FakeTestData(TestDbContext ctx)
    {
        var company = DF.Fake<Company>(ctx);

        // employees
        for (var i = 0; i < 3; i++)
            DF.Fake<AppUser>(ctx, x => x.Company = company);

        // categories and items
        for(var i = 0; i < 3; i++)
        {
            var parent = DF.Fake<Category>(ctx, x => x.Company = company);
            for(var j = 0; j < 3; j++)
            {
                var child = DF.Fake<Category>(ctx, x =>
                {
                    x.Company = company;
                    x.ParentCategory = parent;
                });

                for(var k = 0; k < 3; k++)
                    DF.Fake<Item>(ctx, x => x.Category = child);
            }
        }

        ctx.SaveChanges();

        return company.Id;
    }
}
