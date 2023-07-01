namespace Resourcerer.DataAccess.Entities;

public class Element : EntityBase
{
    public Element()
    {
        Excerpts = new HashSet<Excerpt>();
        OldPrices = new HashSet<OldPrice>();
        ElementPurchasedEvents = new HashSet<ElementPurchasedEvent>();
        ElementSoldEvents = new HashSet<ElementSoldEvent>();
    }

    public string? Name { get; set; }
    public double CurrentSellPrice { get; set; }
    
    public Guid? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    
    public Guid UnitOfMeasureId { get; set; }
    public virtual UnitOfMeasure? UnitOfMeasure { get; set; }
    
    public ICollection<Excerpt> Excerpts { get; set; }
    public ICollection<OldPrice> OldPrices { get; set; }
    public ICollection<ElementPurchasedEvent> ElementPurchasedEvents { get; set; }
    public ICollection<ElementSoldEvent> ElementSoldEvents { get; set; }
}

