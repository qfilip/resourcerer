namespace Resourcerer.DataAccess.Entities;

public class InstanceDeliveredEvent : EntityBase
{
    public Guid? InstanceBoughtEventId { get; set; }
    public virtual InstanceBoughtEvent? InstanceBoughtEvent { get; set; }

    public Guid? InstanceSoldEventId { get; set; }
    public virtual InstanceSoldEvent? InstanceSoldEvent { get; set; }
}
