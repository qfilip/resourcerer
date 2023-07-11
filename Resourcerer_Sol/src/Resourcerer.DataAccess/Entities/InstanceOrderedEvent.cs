namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderedEvent : EntityBase
{
    public double UnitsOrdered { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }

    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }

    public Guid? InstanceOrderCancelledEventId { get; set; }
    public virtual InstanceOrderCancelledEvent? InstanceOrderCancelledEvent { get; set; }

    public Guid? InstanceDeliveredEventId { get; set; }
    public virtual InstanceDeliveredEvent? InstanceDeliveredEvent { get; set; }
}
