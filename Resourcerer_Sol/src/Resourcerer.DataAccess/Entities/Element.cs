namespace Resourcerer.DataAccess.Entities;

public class Element : EntityBase
{
    public Element()
    {
        Excerpts = new HashSet<Excerpt>();
        Prices = new HashSet<Price>();
        ElementInstances = new HashSet<Instance>();
        ElementPurchasedEvents = new HashSet<ElementPurchasedEvent>();
        ElementSoldEvents = new HashSet<ElementInstanceSoldEvent>();
    }

    public string? Name { get; set; }
    
    public Guid? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    
    public Guid UnitOfMeasureId { get; set; }
    public virtual UnitOfMeasure? UnitOfMeasure { get; set; }
    
    public ICollection<Excerpt> Excerpts { get; set; }
    public ICollection<Price> Prices { get; set; }
    public ICollection<Instance> ElementInstances { get; set; }
    public ICollection<ElementPurchasedEvent> ElementPurchasedEvents { get; set; }
    public ICollection<ElementInstanceSoldEvent> ElementSoldEvents { get; set; }
}

