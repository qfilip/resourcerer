using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Resourcerer.DataAccess.Entities;

public class ItemProductionOrder : IId<Guid>, IAuditedEntity, ISoftDeletable
{
    public double Quantity { get; set; }
    public string? Reason { get; set; }
    public Guid CompanyId { get; set; }
    
    // relational
    public Guid ItemId { get; set; }
    public virtual Item? Item { get; set; }

    // json
    [NotMapped]
    public Guid[] InstancesUsedIds { get; set; } = Array.Empty<Guid>();
    public ItemProductionStartedEvent? StartedEvent { get; set; }
    public ItemProductionOrderCancelledEvent? CancelledEvent { get; set; }
    public ItemProductionFinishedEvent? FinishedEvent { get; set; }

    // json mapping
    // (Guid/value types, cannot be used with EF Core model builder for Json entities)
    public string InstancesUsedIdsJson
    {
        get => JsonSerializer.Serialize(InstancesUsedIds);
        set
        {
            if (value == null) return;
            InstancesUsedIds = JsonSerializer.Deserialize<Guid[]>(value)!;
        }
    }

    // entity definition
    public Guid Id { get; set; }
    public AuditRecord AuditRecord { get; set; } = new();
    public eEntityStatus EntityStatus { get; set; }
}

