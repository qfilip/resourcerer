using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.AuthService;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Contexts;

public partial class AppDbContext : DbContext
{
    private readonly AppDbIdentity _appDbIdentity;

    public AppDbContext(DbContextOptions<AppDbContext> options, AppDbIdentity appDbIdentity) : base(options)
	{
        _appDbIdentity = appDbIdentity;
    }

	public virtual DbSet<AppUser> AppUsers { get; set; }
    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Excerpt> Excerpts { get; set; }
    public virtual DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
    public virtual DbSet<Price> Prices { get; set; }
    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<Instance> Instances { get; set; }


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
                added.CreatedBy = _appDbIdentity.User.Id;
                added.ModifiedBy = _appDbIdentity.User.Id;

            }
            else if (entry.State == EntityState.Modified && entry.Entity is EntityBase modded)
            {
                modded.ModifiedAt = now;
                modded.CreatedBy = _appDbIdentity.User.Id;
                modded.ModifiedBy = _appDbIdentity.User.Id;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}