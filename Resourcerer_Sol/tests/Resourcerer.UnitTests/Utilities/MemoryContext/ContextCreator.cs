using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Mocks;
using Resourcerer.Logic.Queries.Mocks;

namespace Resourcerer.UnitTests.Utilities.MemoryContext;

public class ContextCreator : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TestDbContext> _options;
    public ContextCreator()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new TestDbContext(_options);
        if(context.Database.EnsureCreated())
        {
            SeedMockData(context);
        }
    }
    public IAppDbContext GetTestDbContext()
    {
        return new TestDbContext(_options);
    }

    private void SeedMockData(TestDbContext context)
    {
        var getHandler = new GetMockedDatabaseData.Handler();
        var dbData = getHandler.Handle(new Unit()).GetAwaiter().GetResult().Object!;

        var seedHandler = new SeedMockData.Handler(context);
        seedHandler.Handle(dbData).GetAwaiter().GetResult();
    }
    
    public void Dispose()
    {
        _connection.Dispose();
    }
}
