namespace Resourcerer.DataAccess.Entities;

public class Excerpt : AppDbEntity
{
    public double Quantity { get; set; }

    // relational
    public Guid CompositeId { get; set; }
    public Item? Composite { get; set; }
    
    public  Guid ElementId { get; set; }
    public Item? Element { get; set; }

}

