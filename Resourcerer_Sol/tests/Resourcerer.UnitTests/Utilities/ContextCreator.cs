using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;

namespace Resourcerer.UnitTests.Utilities;

public class ContextCreator: IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;
    public ContextCreator(Func<IAppDbContext, Task> seeder)
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new AppDbContext(_options);
        if (context.Database.EnsureCreated())
        {
            seeder(context).GetAwaiter().GetResult();
        }
    }
    public IAppDbContext GetTestDbContext()
    {
        return new AppDbContext(_options);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
