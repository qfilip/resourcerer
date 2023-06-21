namespace Resourcerer.DataAccess.Entities;
public class ElementPurchasedEvent : EntityBase
{
    public int UnitsBought { get; set; }
    public double PriceByUnit { get; set; }
    public UnitOfMeasure? UnitOfMeasure { get; set; }

    public Guid ElementId { get; set; }
    public virtual Element? Element { get; set; }
}

