using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Contexts;

public partial class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public virtual DbSet<AppUser> AppUsers { get; set; }
    
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Excerpt> Excerpts { get; set; }
    public virtual DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
    public virtual DbSet<Price> Prices { get; set; }
    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<Instance> Instances { get; set; }
    public virtual DbSet<InstanceBoughtEvent> InstanceBoughtEvents { get; set; }
    public virtual DbSet<InstanceSoldEvent> InstanceSoldEvents { get; set; }
    public virtual DbSet<InstanceCancelledEvent> InstanceCancelledEvents { get; set; }
    public virtual DbSet<InstanceDeliveredEvent> InstanceDeliveredEvents { get; set; }
    public virtual DbSet<InstanceDiscardedEvent> InstanceDiscardedEvents { get; set; }



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