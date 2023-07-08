using Resourcerer.DataAccess.Contexts;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly AppDbContext _testDbContext;
    public TestsBase()
    {
        _testDbContext = new ContextCreator().GetTestDbContext();
    }
}
