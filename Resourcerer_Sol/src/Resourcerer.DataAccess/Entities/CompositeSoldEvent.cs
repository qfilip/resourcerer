namespace Resourcerer.DataAccess.Entities;
public class CompositeSoldEvent : EntityBase
{
    public Guid CompositeId { get; set; }
    public Guid PriceId { get; set; }

    public virtual Composite? Composite { get; set; }
    public virtual Price? Price { get; set; }
}

