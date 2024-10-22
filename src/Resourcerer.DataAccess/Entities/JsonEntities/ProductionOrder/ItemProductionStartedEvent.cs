using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class ItemProductionStartedEvent : IAuditedEntity<ReadOnlyAudit>
{
    public ReadOnlyAudit AuditRecord {  get; set; } = new();
}
