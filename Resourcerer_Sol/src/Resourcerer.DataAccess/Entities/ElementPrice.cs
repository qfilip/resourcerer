namespace Resourcerer.DataAccess.Entities;

public class ElementPrice : EntityBase
{
    public ElementPrice()
    {
        ElementSoldEvents = new HashSet<ElementSoldEvent>();
    }

    public Guid ElementId { get; set; }
    public double Value { get; set; }

    public virtual Element? Element { get; set; }
    public ICollection<ElementSoldEvent> ElementSoldEvents { get; set; }
}
