using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Records;

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
            var entryKey = entry.Entity as IId<Guid>;
            var entryAudit = entry.Entity as IAuditedEntity<Audit>;

            if (entryKey == null || entryAudit == null)
                continue;
            
            if (entry.State == EntityState.Added)
            {
                entryKey.Id = entryKey.Id == Guid.Empty ? Guid.NewGuid() : entryKey.Id;

                entryAudit.AuditRecord.CreatedAt = now;
                entryAudit.AuditRecord.ModifiedAt = now;
                entryAudit.AuditRecord.CreatedBy = _currentUser.Id;
                entryAudit.AuditRecord.ModifiedBy = _currentUser.Id;

            }
            else if (entry.State == EntityState.Modified)
            {
                entryAudit.AuditRecord.ModifiedAt = now;
                entryAudit.AuditRecord.ModifiedBy = _currentUser.Id;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}