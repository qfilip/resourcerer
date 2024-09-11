using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.UnitTests.Utilities;
using SqlForgery;
using System.Text.Json;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly TestDbContext _ctx;
    protected readonly JsonSerializerOptions _serializerOptions;
    protected readonly Forger _forger;

    public TestsBase()
    {
        _ctx = new ContextCreator().GetTestDbContext();
        
        _serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        _forger = new Forger(_ctx, DataFaking.FakingFunctions);
    }

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();
}
