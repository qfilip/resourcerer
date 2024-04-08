using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Services;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

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

        var context = new TestDbContext(_options, new AppIdentityService<AppUser>());
        context.Database.EnsureCreated();
    }
    
    public TestDbContext GetTestDbContext()
    {
        return new TestDbContext(_options, new AppIdentityService<AppUser>());
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
