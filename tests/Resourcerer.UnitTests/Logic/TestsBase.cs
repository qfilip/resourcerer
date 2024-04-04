using FakeItEasy;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly TestDbContext _ctx;

    public TestsBase()
    {
        _ctx = new ContextCreator().GetTestDbContext();
    }

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();
}
