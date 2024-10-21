using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class InstanceReservedEvent : IPkey<Guid>, IAuditedEntity, ISoftDeletable
{
    public Guid ItemProductionOrderId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    // relational
    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }

    // json
    public InstanceReserveCancelledEvent? CancelledEvent { get; set; }
    public InstanceReserveUsedEvent? UsedEvent { get; set; }

    // entity definition
    public Guid Id { get; set; }
    public AuditRecord AuditRecord { get; set; } = new();
    public eEntityStatus EntityStatus { get; set; }
}