using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Contexts;

public partial class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<AppUser> AppUsers { get; set; }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Excerpt> Excerpts { get; set; }
    public DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
    public DbSet<Price> Prices { get; set; }

    public DbSet<Composite> Composites { get; set; }
    public DbSet<CompositeSoldEvent> CompositeSoldEvents { get; set; }
    
    public DbSet<Element> Elements { get; set; }
    public DbSet<ElementPurchasedEvent> ElementPurchasedEvents { get; set; }
    public DbSet<ElementSoldEvent> ElementSoldEvents { get; set; }

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