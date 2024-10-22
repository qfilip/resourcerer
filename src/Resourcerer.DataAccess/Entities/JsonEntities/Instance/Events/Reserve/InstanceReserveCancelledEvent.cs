using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities.JsonEntities;
public class InstanceReserveCancelledEvent : IAuditedEntity<ReadOnlyAudit>
{
    public string? Reason { get; set; }
    public ReadOnlyAudit AuditRecord {  get; set; } = new();
}
