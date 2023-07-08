using Resourcerer.DataAccess.Contexts;
using Resourcerer.Logic.Commands.Composites;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.Commands.Composites;

public class ChangeCompositePriceTests
{
    private readonly AppDbContext _testDbContext;
    private readonly ChangeCompositePrice.Handler _handler;
    public ChangeCompositePriceTests()
    {
        _testDbContext = new ContextCreator().GetTestDbContext();
        _handler = new ChangeCompositePrice.Handler(_testDbContext);
    }
}
