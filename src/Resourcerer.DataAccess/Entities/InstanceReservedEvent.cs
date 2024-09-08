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
    public InstanceReserveCancelledEvent? CancelledEvent { get; set; }
    public InstanceReserveUsedEvent? UsedEvent { get; set; }
}