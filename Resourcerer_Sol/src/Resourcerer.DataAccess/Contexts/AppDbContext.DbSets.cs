using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Contexts;

public partial class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppEvent> AppEvents { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Composite> Composites { get; set; }
    public DbSet<Element> Elements { get; set; }
    public DbSet<Excerpt> Excerpts { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
}

