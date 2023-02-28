namespace Resourcerer.DataAccess.Entities;

public class Element : EntityBase
{
    public Element()
    {
        Excerpts = new HashSet<Excerpt>();
        ElementPurchasedEvents = new HashSet<ElementPurchasedEvent>();
    }

    public string? Name { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitOfMeasureId { get; set; }

    public virtual Category? Category { get; set; }
    public virtual UnitOfMeasure? UnitOfMeasure { get; set; }
    public ICollection<Excerpt> Excerpts { get; set; }
    public ICollection<ElementPurchasedEvent> ElementPurchasedEvents { get; set; }
}

