using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class ItemProductionOrderCancelledEvent : IAuditedEntity<ReadOnlyAudit>
{
    public string? Reason { get; set; }
    public ReadOnlyAudit AuditRecord {  get; set; } = new();
}
