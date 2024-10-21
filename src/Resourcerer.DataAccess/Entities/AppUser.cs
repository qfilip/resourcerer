using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;
public class AppUser : IId<Guid>, IAuditedEntity, ISoftDeletable
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public string? PasswordHash { get; set; }
    public string? Permissions { get; set; }

    // relational
    public Guid CompanyId { get; set; }
    public virtual Company? Company { get; set; }

    // entity definition
    public Guid Id { get; set; }
    public AuditRecord AuditRecord { get; set; } = new();
    public eEntityStatus EntityStatus { get; set; }
}