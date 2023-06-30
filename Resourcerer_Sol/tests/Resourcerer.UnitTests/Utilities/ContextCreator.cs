using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Mocks;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Mocks;
using Resourcerer.Logic.Queries.Mocks;

namespace Resourcerer.UnitTests.Utilities;

public class ContextCreator : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;
    public ContextCreator(bool seedEvents)
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new AppDbContext(_options);
        if (context.Database.EnsureCreated())
        {
            SeedMockData(context, seedEvents);
        }
    }
    public IAppDbContext GetTestDbContext()
    {
        return new AppDbContext(_options);
    }

    private void SeedMockData(AppDbContext context, bool seedEvents)
    {
        IRequestHandler<Unit, DatabaseData> getMocksHandler =
            seedEvents ?
            new GetMockedDatabaseData.Handler() :
            new GetMockedNonEventDatabaseData.Handler();

        var dbData = getMocksHandler.Handle(new Unit()).GetAwaiter().GetResult().Object!;

        var seedMocksHandler = new SeedMockData.Handler(context);
        seedMocksHandler.Handle(dbData).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
