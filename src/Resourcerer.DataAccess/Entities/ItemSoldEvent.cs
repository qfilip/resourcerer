namespace Resourcerer.DataAccess.Entities;

public class ItemSoldEvent : EntityBase
{
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    
    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }

    public Guid? ItemSellCancelledEventId { get; set; }
    public virtual ItemSellCancelledEvent? ItemSellCancelledEvent { get; set; }

    public Guid? ItemDeliveredEventId { get; set; }
    public virtual ItemDeliveredEvent? ItemDeliveredEvent { get; set; }
}
