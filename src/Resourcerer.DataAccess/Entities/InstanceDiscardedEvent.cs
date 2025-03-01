using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class InstanceDiscardedEvent : IId<Guid>, IAuditedEntity<Audit>
{
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    // relational
    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }

    // entity definition
    public Guid Id { get; set; }
    public Audit AuditRecord { get; set; } = new();
}
