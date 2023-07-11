namespace Resourcerer.DataAccess.Entities;

public class Excerpt : EntityBase
{
    public Guid CompositeId { get; set; }
    public Item? Composite { get; set; }
    
    public  Guid ElementId { get; set; }
    public virtual Item? Element { get; set; }

    public double Quantity { get; set; }
}

