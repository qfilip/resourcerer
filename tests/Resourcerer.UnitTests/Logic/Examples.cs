using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;

namespace Resourcerer.UnitTests.Logic;

public class Examples : TestsBase
{
    [Fact(Skip = "demonstration")]
    public void FakingData()
    {
        var company = DF.Fake<Company>(_ctx, x => x.Name = "acme inc");
        var instance = DF.Fake<Instance>(_ctx, x => x.OwnerCompany = company);

        _ctx.SaveChanges();

        var instanceCompanyName = _ctx.Instances
            .Select(x => new { x.Id, x.OwnerCompany!.Name })
            .First(x => x.Id == instance.Id)
            .Name;
    }
}
