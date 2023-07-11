using FakeItEasy;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly AppDbContext _testDbContext;
    public TestsBase()
    {
        _testDbContext = new ContextCreator().GetTestDbContext();
    }

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();

    [Fact]
    public void Ooh()
    {
        Mocker.MockDbData(_testDbContext);
        _testDbContext.SaveChanges();

        var x = 0;
    }
}
