namespace Resourcerer.DataAccess.Entities;

public class Excerpt : EntityBase
{
    public Guid CompositeId { get; set; }
    public Composite? Composite { get; set; }
    
    public  Guid ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public double Quantity { get; set; }
}

