namespace Resourcerer.DataAccess.Entities;

public class ElementSoldEvent : EntityBase
{
    public double UnitsSold { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public UnitOfMeasure? UnitOfMeasure { get; set; }

    public Guid ElementId { get; set; }
    public virtual Element? Element { get; set; }
}
