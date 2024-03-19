using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Resourcerer.DataAccess.Entities;

public class ItemProductionOrder : EntityBase
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
    
    [NotMapped]
    public ItemProductionStartedEvent? StartedEvent { get; set; }
    
    [NotMapped]
    public ItemProductionOrderCancelledEvent? CanceledEvent { get; set; }
    
    [NotMapped]
    public ItemProductionFinishedEvent? FinishedEvent { get; set; }

    // json mapping
    public string InstancesUsedIdsJson
    {
        get => JsonSerializer.Serialize(InstancesUsedIds);
        set
        {
            if (value == null) return;
            InstancesUsedIds = JsonSerializer.Deserialize<Guid[]>(value)!;
        }
    }
    public string StartedEventJson
    {
        get => JsonSerializer.Serialize(StartedEvent);
        set
        {
            if (value == null) return;
            StartedEvent = JsonSerializer.Deserialize<ItemProductionStartedEvent>(value)!;
        }
    }

    public string CanceledEventJson
    {
        get => JsonSerializer.Serialize(CanceledEvent);
        set
        {
            if (value == null) return;
            CanceledEvent = JsonSerializer.Deserialize<ItemProductionOrderCancelledEvent>(value)!;
        }
    }

    public string FinishedEventJson
    {
        get => JsonSerializer.Serialize(FinishedEvent);
        set
        {
            if (value == null) return;
            FinishedEvent = JsonSerializer.Deserialize<ItemProductionFinishedEvent>(value)!;
        }
    }
}
