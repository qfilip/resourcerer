using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Microsoft.Data.Sqlite;

namespace Resourcerer.UnitTests.Utilities.MemoryContext;

public partial class TestDbContext : DbContext, IAppDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Excerpt> Excerpts { get; set; }
    public virtual DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
    public virtual DbSet<Price> Prices { get; set; }

    public virtual DbSet<Composite> Composites { get; set; }
    public virtual DbSet<CompositeSoldEvent> CompositeSoldEvents { get; set; }

    public virtual DbSet<Element> Elements { get; set; }
    public virtual DbSet<ElementPurchasedEvent> ElementPurchasedEvents { get; set; }
    public virtual DbSet<ElementSoldEvent> ElementSoldEvents { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries();

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State != EntityState.Added && entry.Entity is EntityBase added)
            {
                added.Id = Guid.NewGuid();
                added.CreatedAt = now;
                added.ModifiedAt = now;
            }
            else if (entry.State != EntityState.Modified && entry.Entity is EntityBase modded)
            {
                modded.ModifiedAt = now;
            }

        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
