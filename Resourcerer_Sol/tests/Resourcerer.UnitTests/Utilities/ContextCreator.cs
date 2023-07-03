using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;

namespace Resourcerer.UnitTests.Utilities;

public class ContextCreator: IDisposable
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

    private void SeedMockData(AppDbContext context, bool seedEvents = false)
    {
        CarpenterDbMock.SeedAsync(context).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
