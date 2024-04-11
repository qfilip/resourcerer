using Resourcerer.DataAccess.Entities.JsonEntities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Resourcerer.DataAccess.Entities;

public class InstanceReservedEvent : AppDbEntity
{
    public Guid ItemProductionOrderId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    // relational
    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }

    // json
    [NotMapped]
    public InstanceReserveCancelledEvent? CancelledEvent { get; set; }
    [NotMapped]
    public InstanceReserveUsedEvent? UsedEvent { get; set; }

    public string CancelledEventJson
    {
        get => JsonSerializer.Serialize(CancelledEvent);
        private set
        {
            if (value == null) return;
            CancelledEvent = JsonSerializer.Deserialize<InstanceReserveCancelledEvent>(value);
        }
    }
    public string UsedEventJson
    {
        get => JsonSerializer.Serialize(UsedEvent);
        private set
        {
            if (value == null) return;
            UsedEvent = JsonSerializer.Deserialize<InstanceReserveUsedEvent>(value);
        }
    }
}