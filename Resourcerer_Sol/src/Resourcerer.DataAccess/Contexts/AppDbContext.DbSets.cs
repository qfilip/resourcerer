using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Contexts;

public partial class AppDbContext : DbContext, IAppDbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public virtual DbSet<AppUser> AppUsers { get; set; }
    
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Excerpt> Excerpts { get; set; }
    public virtual DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
    public virtual DbSet<Price> Prices { get; set; }
    public virtual DbSet<Instance> Instances { get; set; }

    public virtual DbSet<Composite> Composites { get; set; }
    public virtual DbSet<CompositeSoldEvent> CompositeSoldEvents { get; set; }
    
    public virtual DbSet<Element> Elements { get; set; }
    public virtual DbSet<ElementPurchasedEvent> ElementPurchasedEvents { get; set; }
    public virtual DbSet<ElementPurchaseCancelledEvent> ElementPurchaseCancelledEvents { get; set; }
    public virtual DbSet<ElementDeliveredEvent> ElementDeliveredEvents { get; set; }
    public virtual DbSet<ElementDiscardedEvent> ElementDiscardedEvents { get; set; }
    

    public virtual DbSet<ElementSoldEvent> ElementSoldEvents { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries();

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added && entry.Entity is EntityBase added)
            {
                added.Id = added.Id == Guid.Empty ? Guid.NewGuid() : added.Id;
                added.CreatedAt = now;
                added.ModifiedAt = now;
            }
            else if (entry.State == EntityState.Modified && entry.Entity is EntityBase modded)
            {
                modded.ModifiedAt = now;
            }

        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> BaseSaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync();
    }
}