namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderCancelledEvent : EntityBase
{
    public string? Reason { get; set; }
    public double RefundedAmount { get; set; }
}
