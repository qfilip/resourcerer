namespace Resourcerer.DataAccess.Entities;

public class Excerpt : EntityBase
{
    public Guid CompositeId { get; set; }
    public Guid ElementId { get; set; }
    
    public virtual Composite? Composite { get; set; }
    public virtual Element? Element { get; set; }

    public double Quantity { get; set; }
}

