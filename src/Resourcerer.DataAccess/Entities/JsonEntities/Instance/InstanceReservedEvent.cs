using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.DataAccess.Entities;

public class InstanceReservedEvent : JsonEntityBase
{
    public Guid ProductionOrderId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    public InstanceReserveCancelledEvent? CancelledEvent { get; set; }
    public InstanceReserveUsedEvent? UsedEvent { get; set; }
}

public class InstanceReserveCancelledEvent : JsonEntityBase
{
    public string? Reason { get; set; }
}

public class InstanceReserveUsedEvent : JsonEntityBase
{

}