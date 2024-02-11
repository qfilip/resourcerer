using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderCancelledEvent : JsonEntityBase
{
    public string? Reason { get; set; }
    public double RefundedAmount { get; set; }
}
