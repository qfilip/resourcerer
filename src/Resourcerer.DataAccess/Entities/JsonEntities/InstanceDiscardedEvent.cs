namespace Resourcerer.DataAccess.Entities;

public class InstanceDiscardedEvent : EntityBase
{
    public string? Owner { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }
}
