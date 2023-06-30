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
    public DbSet<Composite> Composites { get; set; }
    public DbSet<CompositeSoldEvent> CompositeSoldEvents { get; set; }
    public DbSet<Element> Elements { get; set; }
    public DbSet<ElementPurchasedEvent> ElementPurchasedEvents { get; set; }
    public DbSet<ElementSoldEvent> ElementSoldEvents { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> BaseSaveChangesAsync(CancellationToken cancellationToken = default);
}
