namespace Resourcerer.DataAccess.Entities;

public class InstanceSoldEvent : EntityBase
{
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    
    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }

    public Guid? InstanceRequestCancelledEventId { get; set; }
    public virtual InstanceCancelledEvent? InstanceRequestCancelledEvent { get; set; }

    public Guid? InstanceRequestDeliveredEventId { get; set; }
    public virtual InstanceDeliveredEvent? InstanceRequestDeliveredEvent { get; set; }
}
