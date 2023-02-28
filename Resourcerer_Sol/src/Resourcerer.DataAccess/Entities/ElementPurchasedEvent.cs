namespace Resourcerer.DataAccess.Entities;
public class ElementPurchasedEvent : EntityBase
{
    public Guid ElementId { get; set; }
    public int NumOfUnits { get; set; }
    public double UnitPrice { get; set; }

    public virtual Element? Element { get; set; }
}

