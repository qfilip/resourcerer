namespace Resourcerer.DataAccess.Entities;
public class CompositeSoldEvent : EntityBase
{
    public double UnitsSold { get; set; }
    public double UnitPrice { get; set; }

    public Guid CompositeId { get; set; }
    public virtual Composite? Composite { get; set; }
}

