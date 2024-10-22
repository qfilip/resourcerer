using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderSentEvent : IAuditedEntity<ReadOnlyAudit>
{
    public ReadOnlyAudit AuditRecord { get; set; } = new();
}