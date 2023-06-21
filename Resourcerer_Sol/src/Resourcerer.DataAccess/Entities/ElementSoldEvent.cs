namespace Resourcerer.DataAccess.Entities;

public class ElementSoldEvent : EntityBase
{
    public int UnitsSold { get; set; }
    public double PriceByUnit { get; set; }
    public UnitOfMeasure? UnitOfMeasure { get; set; }

    public Guid ElementId { get; set; }
    public virtual Element? Element { get; set; }
}
