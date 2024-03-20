using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.DataAccess.Entities;

public class InstanceReservedEvent : AppDbEntity
{
    public Guid ItemProductionOrderId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    // json
    public InstanceReserveCancelledEvent? CancelledEvent { get; set; }
    public InstanceReserveUsedEvent? UsedEvent { get; set; }

    // relational
    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }
}

public class InstanceReserveCancelledEvent : AppDbJsonField
{
    public string? Reason { get; set; }
}

public class InstanceReserveUsedEvent : AppDbJsonField
{

}