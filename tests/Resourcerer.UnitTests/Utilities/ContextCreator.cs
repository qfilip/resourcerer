using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Api.Services.Auth;
using Resourcerer.DataAccess.Contexts;

namespace Resourcerer.UnitTests.Utilities;

public class ContextCreator: IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;
    public ContextCreator()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new TestDbContext(_options, new AppIdentityService());
        context.Database.EnsureCreated();
    }
    
    public TestDbContext GetTestDbContext()
    {
        return new TestDbContext(_options, new AppIdentityService());
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
