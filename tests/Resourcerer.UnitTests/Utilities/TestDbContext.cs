using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;

namespace Resourcerer.UnitTests.Utilities;

public class TestDbContext : AppDbContext
{
    public TestDbContext(
        DbContextOptions<AppDbContext> options,
        IAppIdentityService<AppIdentity> _appIdentityService) : base(options, ContextCreator.SystemIdentity)
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
