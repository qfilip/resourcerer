using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Logic.V1.Functions.Instances;

public class GetUnitsInStockTests : TestsBase
{
    private readonly Func<Instance, double> _sut;
    private readonly Guid _staticId;
    public GetUnitsInStockTests()
    {
        _sut = Resourcerer.Logic.V1.Functions.Instances.GetUnitsInStock;
        _staticId = Guid.NewGuid();
    }

    [Fact]
    public void Todo() => Assert.Fail("Implement tests");
}
