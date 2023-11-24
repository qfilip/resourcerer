namespace Resourcerer.DataAccess.Entities;

public class InstanceCancelledEvent : EntityBase
{
    public string? Reason { get; set; }
    
    public Guid? InstanceBoughtEventId { get; set; }
    public virtual InstanceBoughtEvent? InstanceBoughtEvent { get; set; }

    public Guid? InstanceSoldEventId { get; set; }
    public virtual InstanceSoldEvent? InstanceSoldEvent { get; set; }
}
