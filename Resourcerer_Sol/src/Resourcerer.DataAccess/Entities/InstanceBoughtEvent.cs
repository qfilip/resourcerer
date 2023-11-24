namespace Resourcerer.DataAccess.Entities;

public class InstanceBoughtEvent : EntityBase
{
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }

    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }

    public Guid? InstanceCancelledEventId { get; set; }
    public virtual InstanceCancelledEvent? InstanceCancelledEvent { get; set; }

    public Guid? InstanceDeliveredEventId { get; set; }
    public virtual InstanceDeliveredEvent? InstanceDeliveredEvent { get; set; }
}
