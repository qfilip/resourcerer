using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Contexts;

public interface IAppDbContext
{
    public DbSet<AppUser> AppUsers { get; set; }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Excerpt> Excerpts { get; set; }
    public DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Instance> Instances { get; set; }

    public DbSet<Composite> Composites { get; set; }
    public DbSet<CompositeSoldEvent> CompositeSoldEvents { get; set; }

    public DbSet<Element> Elements { get; set; }
    public DbSet<ElementPurchasedEvent> ElementPurchasedEvents { get; set; }
    public DbSet<ElementPurchaseCancelledEvent> ElementPurchaseCancelledEvents { get; set; }
    public DbSet<ElementDeliveredEvent> ElementDeliveredEvents { get; set; }
    public DbSet<ElementInstanceDiscardedEvent> ElementDiscardedEvents { get; set; }
    public DbSet<ElementInstanceSoldEvent> ElementSoldEvents { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> BaseSaveChangesAsync(CancellationToken cancellationToken = default);
}
