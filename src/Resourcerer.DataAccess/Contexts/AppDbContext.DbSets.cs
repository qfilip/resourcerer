using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Services;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Contexts;

public partial class AppDbContext : DbContext
{
    private readonly IAppIdentityService<AppUser> _appIdentityService;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IAppIdentityService<AppUser> appIdentityService) : base(options)
	{
        _appIdentityService = appIdentityService;
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
            if (entry.State == EntityState.Added && entry.Entity is AppDbEntity added)
            {
                added.Id = added.Id == Guid.Empty ? Guid.NewGuid() : added.Id;
                added.CreatedAt = now;
                added.ModifiedAt = now;
                added.CreatedBy = _appIdentityService.Get().Id;
                added.ModifiedBy = _appIdentityService.Get().Id;

            }
            else if (entry.State == EntityState.Modified && entry.Entity is AppDbEntity modded)
            {
                modded.ModifiedAt = now;
                modded.CreatedBy = _appIdentityService.Get().Id;
                modded.ModifiedBy = _appIdentityService.Get().Id;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}