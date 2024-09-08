using System.Text.Json;

namespace Resourcerer.DataAccess.Entities;

public class ItemProductionOrder : AppDbEntity
{
    public double Quantity { get; set; }
    public string? Reason { get; set; }
    public Guid CompanyId { get; set; }
    
    // relational
    public Guid ItemId { get; set; }
    public virtual Item? Item { get; set; }

    // json
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
}

