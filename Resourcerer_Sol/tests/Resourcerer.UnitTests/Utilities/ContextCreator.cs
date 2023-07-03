using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Mocks;
using System.Text.Json;

namespace Resourcerer.UnitTests.Utilities;

public class ContextCreator: IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;
    public ContextCreator(Func<DatabaseData> seeder)
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new AppDbContext(_options);
        if (context.Database.EnsureCreated())
        {
            var dbData = seeder();
            // write to disk if mocks are needed
            var dbDataJson = JsonSerializer.Serialize(dbData);

            context.AppUsers.AddRange(dbData.AppUsers!);
            context.Categories.AddRange(dbData.Categories!);
            context.Excerpts.AddRange(dbData.Excerpts!);
            context.UnitsOfMeasure.AddRange(dbData.UnitsOfMeasure!);
            context.Prices.AddRange(dbData.Prices!);
            context.Composites.AddRange(dbData.Composites!);
            context.CompositeSoldEvents.AddRange(dbData.CompositeSoldEvents!);
            context.Elements.AddRange(dbData.Elements!);
            context.ElementSoldEvents.AddRange(dbData.ElementSoldEvents!);
            context.ElementPurchasedEvents.AddRange(dbData.ElementPurchasedEvents!);

            context.SaveChanges();
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
