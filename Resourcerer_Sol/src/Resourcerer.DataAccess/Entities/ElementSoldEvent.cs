namespace Resourcerer.DataAccess.Entities;

public class ElementSoldEvent : EntityBase
{
    public Guid ElementId { get; set; }
    public Guid PriceId { get; set; }

    public virtual Element? Element { get; set; }
    public virtual ElementPrice? Price { get; set; }
}
