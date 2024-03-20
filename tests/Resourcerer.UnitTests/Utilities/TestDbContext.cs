using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.AuthService;
using Resourcerer.DataAccess.Contexts;

namespace Resourcerer.UnitTests.Utilities;

public class TestDbContext : AppDbContext
{
    public TestDbContext(DbContextOptions<AppDbContext> options, AppDbIdentity appDbIdentity) : base(options, appDbIdentity)
    {
    }

    public void Clear() => ChangeTracker.Clear();

    public override int SaveChanges()
    {
        var result = base.SaveChanges();
        base.ChangeTracker.Clear();

        return result;
    }
}
