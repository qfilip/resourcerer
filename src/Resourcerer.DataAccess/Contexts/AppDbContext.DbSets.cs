using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Contexts;

public partial class AppDbContext : DbContext
{
    private readonly AppUser _currentUser;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        AppUser currentUser) : base(options)
	{
        _currentUser = currentUser ?? new AppUser();
    }

	public virtual DbSet<AppUser> AppUsers { get; set; }
    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Excerpt> Excerpts { get; set; }
    public virtual DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
    public virtual DbSet<Price> Prices { get; set; }
    // items
    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<ItemProductionOrder> ItemProductionOrders { get; set; }
    // instances
    public virtual DbSet<Instance> Instances { get; set; }
    public virtual DbSet<InstanceOrderedEvent> InstanceOrderedEvents { get; set; }
    public virtual DbSet<InstanceReservedEvent> InstanceReservedEvents { get; set; }
    public virtual DbSet<InstanceDiscardedEvent> InstanceDiscardedEvents { get; set; }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries();

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added && entry.Entity is IAuditedEntity added)
            {
                if(entry.Entity is IPkey<Guid> keyed)
                    keyed.Id = keyed.Id == Guid.Empty ? Guid.NewGuid() : keyed.Id;

                added.AuditRecord.CreatedAt = now;
                added.AuditRecord.ModifiedAt = now;
                added.AuditRecord.CreatedBy = _currentUser.Id;
                added.AuditRecord.ModifiedBy = _currentUser.Id;

            }
            else if (entry.State == EntityState.Modified && entry.Entity is IAuditedEntity modded)
            {
                modded.AuditRecord.ModifiedAt = now;
                modded.AuditRecord.ModifiedBy = _currentUser.Id;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}