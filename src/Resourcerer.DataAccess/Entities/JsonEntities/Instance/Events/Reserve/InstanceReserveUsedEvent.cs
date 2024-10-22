using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities.JsonEntities;
public class InstanceReserveUsedEvent : IAuditedEntity<ReadOnlyAudit>
{
    public ReadOnlyAudit AuditRecord {  get; set; } = new();
}
