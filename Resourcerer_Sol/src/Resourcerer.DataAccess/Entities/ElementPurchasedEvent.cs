namespace Resourcerer.DataAccess.Entities;
public class ElementPurchasedEvent : EntityBase
{
    public double UnitsBought { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime ExpectedDeliveryTime { get; set; }
    public UnitOfMeasure? UnitOfMeasure { get; set; }

    public Guid ElementId { get; set; }
    public virtual Element? Element { get; set; }

    public virtual ElementPurchaseCancelledEvent? ElementPurchaseCancelledEvent { get; set; }
    public virtual ElementDeliveredEvent? ElementDeliveredEvent { get; set; }
}

