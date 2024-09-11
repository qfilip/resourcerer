using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;
using SqlForgery;

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
        var companyId = FakeTestData(_forger, _ctx);

        var result = _sut.Handle(companyId).Await();
        
        Assert.NotNull(result);
    }

    private static Guid FakeTestData(Forger forger, TestDbContext ctx)
    {
        var company = forger.Fake<Company>();

        // employees
        for (var i = 0; i < 3; i++)
            forger.Fake<AppUser>(x => x.Company = company);

        // categories and items
        for(var i = 0; i < 3; i++)
        {
            var parent = forger.Fake<Category>(x => x.Company = company);
            for(var j = 0; j < 3; j++)
            {
                var child = forger.Fake<Category>(x =>
                {
                    x.Company = company;
                    x.ParentCategory = parent;
                });

                for(var k = 0; k < 3; k++)
                    forger.Fake<Item>(x => x.Category = child);
            }
        }

        ctx.SaveChanges();

        return company.Id;
    }
}
