using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class Price : IId<Guid>, IAuditedEntity, ISoftDeletable
{
    public double UnitValue { get; set; }

    // relational
    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    // entity definition
    public Guid Id { get; set; }
    public AuditRecord AuditRecord { get; set; } = new();
    public eEntityStatus EntityStatus { get; set; }
}
