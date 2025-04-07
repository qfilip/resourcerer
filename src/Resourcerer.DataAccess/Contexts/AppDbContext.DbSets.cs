using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Records;
using Resourcerer.Identity.Models;

namespace Resourcerer.DataAccess.Contexts;

public partial class AppDbContext : DbContext
{
    private readonly AppIdentity _identity;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        AppIdentity identity) : base(options)
	{
        _identity = identity;
    }

	public virtual DbSet<ExampleEntity> Examples { get; set; }

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
                entryAudit.AuditRecord.CreatedBy = _identity.Id;
                entryAudit.AuditRecord.ModifiedBy = _identity.Id;

            }
            else if (entry.State == EntityState.Modified)
            {
                entryAudit.AuditRecord.ModifiedAt = now;
                entryAudit.AuditRecord.ModifiedBy = _identity.Id;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}