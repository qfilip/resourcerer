using FakeItEasy;
using Microsoft.Extensions.Logging;
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

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();
}
