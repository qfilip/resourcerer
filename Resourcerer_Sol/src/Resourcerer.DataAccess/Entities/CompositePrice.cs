namespace Resourcerer.DataAccess.Entities;

public class CompositePrice : EntityBase
{
    public CompositePrice()
    {
        CompositeSoldEvents = new HashSet<CompositeSoldEvent>();
    }

    public Guid CompositeId { get; set; }
    public double Value { get; set; }

    public virtual Composite? Composite { get; set; }
    public ICollection<CompositeSoldEvent> CompositeSoldEvents { get; set; }
}